using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace PropertyPanelLibrary.BasicElements;

public class SUIDropdownListContainer : UIElementGroup
{
    private class DropdownOption : UIElementGroup
    {
        internal const int ElementHeight = 32;
        private readonly SUIDropdownListContainer _container;
        private readonly UITextView _labelElement; // 没有SlideText，暂时用这个代替了

        public DropdownOption(string name, SUIDropdownListContainer father)
        {
            SetSize(0f, ElementHeight, 1, 0);
            BorderRadius = new Vector4(8f);
            Border = 3;
            _container = father; // 叠！

            LeftMouseDown += LeftMouseDownEvent;

            _labelElement = new UITextView()
            {
                Text = name
            };
            _labelElement.SetLeft(8);
            _labelElement.SetTop(0, 0, 0.5f);
            _labelElement.Join(this);
        }

        private void LeftMouseDownEvent(UIView sender, UIMouseEvent evt)
        {
            _container.OnOptionSelected(_labelElement.Text);
        }

        private static Color BeginBgColor => SUIColor.Background * .5f;
        private static Color EndBgColor => SUIColor.Highlight * .5f;

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            BackgroundColor = HoverTimer.Lerp(Color.Transparent, EndBgColor);
            if (_container._currentSelectedLabel == _labelElement.Text)
                BackgroundColor = HoverTimer.Lerp(BeginBgColor, EndBgColor);

            if (IsMouseHovering)
                _container.HoverOnOptionCallback?.Invoke(_labelElement.Text);
            base.Draw(gameTime, spriteBatch);
        }
    }

    /// <summary>
    /// 调用下拉框的UI元素
    /// </summary>
    public UIView DropdownCaller; // TODO 检查调用

    private readonly SUIScrollView _dropdownList;
    private string _currentSelectedLabel;

    /// <summary>
    /// 某个选项被选中时调用
    /// </summary>
    public Action<string> OptionSelectedCallback;

    /// <summary>
    /// 绘制时调用，用于显示Tooltip
    /// </summary>
    public Action DrawCallback;

    /// <summary>
    /// 鼠标悬停在某个选项上时调用，用于显示Tooltip，执行后于DrawCallback
    /// </summary>
    public Action<string> HoverOnOptionCallback;

    public bool Enabled
    {
        get => !IgnoreMouseInteraction;
        set
        {
            IgnoreMouseInteraction = !value;

            if (value == false)
            {
                DropdownCaller = null;
                _animationTimer.StartReverseUpdate();
            }
        }
    }

    private readonly SUIBlurMaskView _blurMask;

    public SUIDropdownListContainer()
    {
        SetSize(0, 0, 1, 1);
        SetPadding(0);
        Enabled = false;
        OverflowHidden = true;
        _blurMask = new SUIBlurMaskView()
        {
            BorderRadius = new Vector4(10f),
        };
        _blurMask.Join(this);
        _dropdownList = new SUIScrollView()
        {
            OverflowHidden = true,
            BorderRadius = new Vector4(10f),
            BackgroundColor = Color.Black * .4f,
            IgnoreMouseInteraction = false
        };
        _dropdownList.SetPadding(4);
        _dropdownList.Join(_blurMask);
    }

    public void SetCurrentPosition(float x, float y, float width, int count)
    {
        var bounds = Bounds;
        // 决定高度
        float containerBottom = bounds.Bottom - 10;
        float optionsHeight = DropdownOption.ElementHeight * count + 10;
        float maximumHeight = 340; // 不要太长，限制高度
        float height = MathF.Min(optionsHeight, maximumHeight);
        _dropdownList.SetSize(width, height);

        // 决定位置，x和y给的是屏幕坐标
        _mouseYClicked = y * Main.UIScale;
        // 限制在UI界面内
        x -= bounds.X;
        float bottom = y + height;
        if (bottom > containerBottom)
        {
            // 计算屏幕坐标
            var scrnY = containerBottom - height;
            // 计算对父元素的相对坐标
            var relativeY = scrnY - bounds.Y;
            _blurMask.SetLeft(x);
            _blurMask.SetTop(relativeY);
        }
        else
        {
            y -= bounds.Y;
            _blurMask.SetLeft(x);
            _blurMask.SetTop(y);
        }
    }

    public void BuildDropdownList(float x, float y, float width, string[] options, string currentLabel, UIView caller)
    {
        int count = options.Length;
        _dropdownList.Container.RemoveAllChildren();
        DropdownCaller = caller;
        SetCurrentPosition(x, y, width, count);

        // 添加元素
        foreach (string label in options)
        {
            var option = new DropdownOption(label, this);
            option.OnUpdate += delegate
            {
                if (option.IsMouseHovering)
                    Main.LocalPlayer.mouseInterface = true;
            };

            option.Join(_dropdownList.Container);
        }

        // 使其有效 & 最后处理
        _animationTimer.StartUpdate();
        _currentSelectedLabel = currentLabel;
        Enabled = true;
    }

    public void OnOptionSelected(string option)
    {
        Enabled = false;
        OptionSelectedCallback?.Invoke(option);
    }

    public override void OnLeftMouseDown(UIMouseEvent evt)
    {
        Enabled = false;
        SoundEngine.PlaySound(SoundID.MenuTick);
        base.OnLeftMouseDown(evt);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        base.UpdateStatus(gameTime);

        if (!IsMouseHovering)
            return;

        Main.LocalPlayer.mouseInterface = true;
    }

    #region Animation - 出现动画

    private float _mouseYClicked; // 鼠标点击时的Y坐标
    private readonly AnimationTimer _animationTimer = new AnimationTimer(3f);

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (!Enabled && _animationTimer.IsForwardCompleted)
            return;

        _animationTimer.Update(gameTime);
        DrawCallback?.Invoke();

        base.Draw(gameTime, spriteBatch);
    }

    public override Rectangle GetClippingRectangle(SpriteBatch spriteBatch)
    {
        var dimensionsRect = _blurMask.Bounds;
        float top = dimensionsRect.Y * Main.UIScale;
        float bottom = dimensionsRect.Bottom * Main.UIScale;
        float maxOffset = Math.Max(Math.Abs(top - _mouseYClicked), Math.Abs(bottom - _mouseYClicked));
        float offset = _animationTimer.Lerp(0, maxOffset);
        int screenWidth = (int)(Main.screenWidth * Main.UIScale);
        var clippingRectangle = new Rectangle(0, (int)(_mouseYClicked - offset), screenWidth, (int)offset * 2);
        // SDFRectangle.DrawNoBorder(new(0, (int)(_mouseYClicked - offset)), new(screenWidth, (int)offset * 2), new(8f), Color.Red * .5f, SilkyUI.TransformMatrix);
        Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
        Rectangle adjustedClippingRectangle =
    Rectangle.Intersect(clippingRectangle, scissorRectangle);
        return adjustedClippingRectangle;
    }

    public override bool ContainsPoint(Vector2 point)
    {
        return GetClippingRectangle(Main.spriteBatch).Contains(point.ToPoint());
    }

    #endregion Animation - 出现动画
}

[RegisterUI("Vanilla: Mouse Text", $"{nameof(PropertyPanelLibrary)}:{nameof(DropdownListUI)}", 214514)]
public class DropdownListUI : BaseBody
{
    public static SUIDropdownListContainer Container { get; private set; }
    public static DropdownListUI Instance { get; private set; }

    protected override void OnInitialize()
    {
        Instance = this;
        BorderColor = default;
        BackgroundColor = default;
        IgnoreMouseInteraction = true;
        SetSize(Main.screenWidth, Main.screenHeight);
        Container = new SUIDropdownListContainer();
        Container.Join(this);

        base.OnInitialize();
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        //var bounds = Container.Bounds;
        // Main.NewText(bounds);
        base.UpdateStatus(gameTime);
    }
}