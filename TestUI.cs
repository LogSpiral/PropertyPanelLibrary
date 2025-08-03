using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Fillers;
using SilkyUIFramework;
using SilkyUIFramework.Animation;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using SilkyUIFramework.Graphics2D;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary;
public class PropertyPanelShowcaseConfig
{
    #region Basic

    public bool SomeBool { get; set; } = true;
    public sbyte SomeSByte { get; set; } = -37;
    public byte SomeByte { get; set; } = 42;
    public short SomeShort { get; set; } = 11111;
    public ushort SomeUShort { get; set; } = 23333;
    public int SomeInt { get; set; } = 114514;
    public uint SomeUInt { get; set; } = 1919810;
    public long SomeLong { get; set; } = -31415926;
    public ulong SomeULong { get; set; } = 27182818;
    public nint SomeNInt { get; set; } = -1111111111;
    public nuint SomeNUInt { get; set; } = 2222222222;
    public float SomeFloat { get; set; } = 0.618f;
    public double SomeDouble { get; set; } = 0.5771;
    public decimal SomeDecimal { get; set; } = 0.495m;
    public string SomeString { get; set; } = "推一辈子地底蔷薇";

    public BindingFlags SomeEnum { get; set; } = BindingFlags.Instance;


    public ItemDefinition SomeItem { get; set; } = new();

    public NPCDefinition SomeNPC { get; set; } = new(NPCID.SkeletronHead);
    #endregion

    #region Object
    public Vector2 SomeVector2 { get; set; } = new(223, 514);

    public Vector3 SomeVector3 { get; set; } = new(223, 214, 514);
    public Vector4 SomeVector4 { get; set; } = new(514, 514, 514, 514);
    public Anchor SomeAnchor { get; set; } = new(4, 0.1f, 0);

    public Dimension SomeDimension { get; set; } = new(4, 0.1f);
    public Margin SomeMargin { get; set; } = new(4, 0.1f, 0, 1f);

    public Color SomeColor { get; set; } = Color.Green;
    public Point SomePoint { get; set; } = new(111, 222);
    public Rectangle SomeRectangle { get; set; } = new(0, 1, 2, 3);

    public UIView SomeUIView { get; set; } = new();
    #endregion

    #region Collection
    public bool[] SomeArray { get; set; } = [false, true, false];
    public List<int> SomeList { get; set; } = [5, 1, 4];

    public Dictionary<string, int> SomeDictionary { get; set; } = new()
    {
        { "Koishi", 514 },
        { "Kokoro", 223 },
        { "Satori", 5 },
        { "Flandre", 495 } ,
        { "Cirno",9}
    };

    public HashSet<string> SomeSet { get; set; } =
        [
        "Yabusame",
        "Tsubakura"
        ];
    #endregion

    #region SubConfig
    public PropertyPanelShowcaseConfig subConfig { get; set; }

    #endregion

}

[RegisterUI("Vanilla: Radial Hotbars", $"{nameof(PropertyPanelLibrary)}: {nameof(TestUI)}")]
// [RegisterGlobalUI("MenuUI", 1)]
public class TestUI : BasicBody
{
    public static TestUI Instance { get; private set; }

    protected override void OnInitialize()
    {
        SetLeft(0, 0, 0.5f);
        SetTop(0, 0, 0.5f);
        SetWidth(800, 0);
        SetHeight(600, 0);
        SetPadding(0);
        BackgroundColor = SUIColor.Background * .25f;
        BorderColor = SUIColor.Border;
        BorderRadius = new(8f);
        Instance = this;
        base.OnInitialize();
    }

    public AnimationTimer SwitchTimer { get; init; } = new(3);
    public static bool Active { get; set; }

    public override bool Enabled
    {
        get => Active || !SwitchTimer.IsReverseCompleted;
        set => Active = value;
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (BlurMakeSystem.BlurAvailable)
        {
            if (BlurMakeSystem.SingleBlur)
            {
                var batch = Main.spriteBatch;
                batch.End();
                BlurMakeSystem.KawaseBlur();
                batch.Begin();
            }

            SDFRectangle.SampleVersion(BlurMakeSystem.BlurRenderTarget,
                Bounds.Position * Main.UIScale, Bounds.Size * Main.UIScale, BorderRadius * Main.UIScale, Matrix.Identity);
        }

        base.Draw(gameTime, spriteBatch);
    }

    protected override void UpdateStatus(GameTime gameTime)
    {
        if (Active) SwitchTimer.StartUpdate();
        else SwitchTimer.StartReverseUpdate();

        SwitchTimer.Update(gameTime);

        UseRenderTarget = SwitchTimer.IsUpdating;
        Opacity = SwitchTimer.Lerp(0f, 1f);

        var center = Bounds.Center * Main.UIScale;
        RenderTargetMatrix =
            Matrix.CreateTranslation(-center.X, -center.Y, 0) *
            Matrix.CreateScale(SwitchTimer.Lerp(0.95f, 1f), SwitchTimer.Lerp(0.95f, 1f), 1) *
            Matrix.CreateTranslation(center.X, center.Y, 0);
        base.UpdateStatus(gameTime);
    }

    public static void Open()
    {
        Instance.RemoveAllChildren();
        Active = true;
        SoundEngine.PlaySound(SoundID.MenuOpen);


        //var list = new SUIScrollView();
        //list.SetSize(500, 400);
        //list.FitHeight = true;
        //list.Mask.FitHeight = true;
        //list.Container.FitHeight = true;
        //for (int n = 0; n < 5; n++)
        //{
        //    var view = new UIView();
        //    view.BackgroundColor = Color.Black * .25f;
        //    view.Border = 2f;
        //    view.BorderColor = Color.Black;
        //    view.SetSize(0, Main.rand.NextFloat(30, 120), 1, 0);
        //    list.Container.AppendChild(view);
        //}
        //list.Join(Instance);



#if false
        Instance.FitHeight = true;
        object[] array = [1, "2", BindingFlags.Static, false, new NPCDefinition(NPCID.MoonLordCore),new int[] {1,2,3 }];
        var panel = PropertyPanel.FromObject(array);
        //var panel = PropertyPanel.FromObject(new { SomeInt = 1, SomeBool = false, SomeString = "2333",袜真的是你啊 = "哇哇哇", SomeFlag = BindingFlags.Static,SomeTile = new TileDefinition(TileID.RainbowBrick) });
        panel.Join(Instance);
        panel.Decorator = FitHeightDecorator.Instance;
#else
        Instance.FitHeight = false;

        //panel.Filler = NoneFiller.Instance;
        //panel.Filler = new DesignatedMemberFiller(
        //    ( 
        //    Main.LocalPlayer, 
        //    [
        //        nameof(Player.statLife),
        //        nameof(Player.statLifeMax2), 
        //        nameof(Player.statMana), 
        //        nameof(Player.stoned)
        //    ]));

        var panel = new PropertyPanel();
        panel.SetSize(0, 0, 1, 1);
        var obj = new PropertyPanelShowcaseConfig() { SomeUIView = new() { Left = new(4), Top = new(4), Width = new(0, .5f), Height = new(0, .75f) } };
        panel.Filler = new DesignatedMemberFiller(
            (
            obj,
            [
                nameof(PropertyPanelShowcaseConfig.SomeVector3),
                nameof(PropertyPanelShowcaseConfig.SomeVector4),
                 nameof(PropertyPanelShowcaseConfig.SomeAnchor),
                nameof(PropertyPanelShowcaseConfig.SomeRectangle),
                nameof(PropertyPanelShowcaseConfig.SomeDimension),
                nameof(PropertyPanelShowcaseConfig.SomeMargin),
                nameof(PropertyPanelShowcaseConfig.SomeColor),
                nameof(PropertyPanelShowcaseConfig.SomeUIView)
            ]));

        panel.Join(Instance);

#endif
        // new PropertyPanelShowcaseConfig()
    }


    public static void Close()
    {
        SoundEngine.PlaySound(SoundID.MenuClose);
        Active = false;
    }
}

public class TestUIPlayer : ModPlayer
{
    private static ModKeybind OpenTestUIKeyBind { get; set; }

    public override void Load()
    {
        OpenTestUIKeyBind = KeybindLoader.RegisterKeybind(Mod, nameof(OpenTestUIKeyBind), Keys.N);
        base.Load();
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (OpenTestUIKeyBind.JustPressed)
        {
            if (TestUI.Active)
                TestUI.Close();
            else
                TestUI.Open();
        }
        base.ProcessTriggers(triggersSet);
    }
}
