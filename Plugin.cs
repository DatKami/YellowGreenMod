using IllusionPlugin;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


using System.Collections.Generic;
using System.Linq;
using Xft;


namespace YellowGreenMod
{
  public class Plugin : IEnhancedPlugin, IPlugin
  {
    public string Name
    {
      get { return "Yellow/Green Mod"; }
    }

    public string Version
    {
      get { return "0.0.2"; }
    }

    private PlayerController _playerController;
    private Color _leftColor;
    private Color _rightColor;
    private GreenColorSO _greenColorSO;
    private YellowColorSO _yellowColorSO;
    private Material _blueSaberMat;
    private Material _redSaberMat;
    private LightSwitchEventEffect[] _lightSwitches;
    private ColorNoteVisuals[] _colorNoteVisuals;
    private NoteDebris[] _noteDebris;
    private NoteMeshCutDebris[] _noteMeshCutDebris;
    private XWeaponTrail[] _weaponTrails;
    private BeatEffect[] _beatEffects;
    private int _colorNoteVisualUpdateRate;
    private int _colorNoteVisualUpdateCounter;

    private Plugin Instance = null;
    public string[] Filter { get; }

    public void OnApplicationStart()
    {
      Console.WriteLine("App Start");
      SceneManager.activeSceneChanged += this.SceneManagerOnActiveSceneChanged;
      this._greenColorSO = ScriptableObject.CreateInstance<GreenColorSO>();
      this._yellowColorSO = ScriptableObject.CreateInstance<YellowColorSO>();
      this._leftColor = new Color(1f, 0f, 1f);
      this._rightColor = new Color(0f, 1f, 0f);
      this._colorNoteVisualUpdateRate = 30;
      this._colorNoteVisualUpdateCounter = 0;
    }

    public void OnApplicationQuit()
    {
      Console.WriteLine("App end");
      SceneManager.activeSceneChanged -= this.SceneManagerOnActiveSceneChanged;
    }

    private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
    {
      // Console.WriteLine("A");
      this._playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
      if (this._playerController == null)
        return;
      if (Instance == null)
      {
        Console.WriteLine("B");
        Material[] source = Resources.FindObjectsOfTypeAll<Material>();
        this._blueSaberMat = source.FirstOrDefault((Material x) => x.name == "BlueSaber");
        this._redSaberMat = source.FirstOrDefault((Material x) => x.name == "RedSaber");
        Console.WriteLine("H");
        Instance = this;
      }

      Console.WriteLine("E");
      this._lightSwitches = UnityEngine.Object.FindObjectsOfType<LightSwitchEventEffect>();
      foreach (LightSwitchEventEffect lightSwitch in this._lightSwitches)
      {
        Console.WriteLine("Found lightswitch");
        ReflectionUtil.SetPrivateField(lightSwitch, "_lightColor0", this._yellowColorSO);
        ReflectionUtil.SetPrivateField(lightSwitch, "_lightColor1", this._greenColorSO);
        ReflectionUtil.SetPrivateField(lightSwitch, "_highlightColor0", this._yellowColorSO);
        ReflectionUtil.SetPrivateField(lightSwitch, "_highlightColor1", this._greenColorSO);
        Console.WriteLine("Set Lightswitches");
      }

      Console.WriteLine("C");
      this._redSaberMat.SetColor("_Color", this._leftColor);
      this._blueSaberMat.SetColor("_Color", this._rightColor);
      Console.WriteLine("G");


      this._weaponTrails = UnityEngine.Object.FindObjectsOfType<XWeaponTrail>();
/**
      Console.WriteLine("D");
      this._colorManager = UnityEngine.Object.FindObjectOfType<ColorManager>();
      ReflectionUtil.SetPrivateField(this._colorManager, "_colorA", new Color(1f, .8f, 0f, 1f));
      ReflectionUtil.SetPrivateField(this._colorManager, "_colorB", new Color(0f, 1f, 0f, 1f));
      Console.WriteLine("F");
*/

    }

    public void OnLevelWasLoaded(int level)
    {

    }

    public void OnLevelWasInitialized(int level)
    {

    }

    public Boolean CheckIsUnchangedRed(Color c)
    {
      return c.r > .9f && c.g < .5f && c.b < .5f;
    }

    public Boolean CheckIsUnchangedBlue(Color c) 
    {
      return c.b > .8f && c.r < .5f;
    }

    public void OnUpdate()
    {
      this._noteDebris = UnityEngine.Object.FindObjectsOfType<NoteDebris>();
      this._noteMeshCutDebris = UnityEngine.Object.FindObjectsOfType<NoteMeshCutDebris>();
      this._beatEffects = UnityEngine.Object.FindObjectsOfType<BeatEffect>();

      // rate limit color note visual update to increase performance
      _colorNoteVisualUpdateCounter += 1;
      if (_colorNoteVisualUpdateCounter > _colorNoteVisualUpdateRate) 
      {
        this._colorNoteVisuals = UnityEngine.Object.FindObjectsOfType<ColorNoteVisuals>();

        _colorNoteVisualUpdateCounter = 0;
        Console.WriteLine(_colorNoteVisualUpdateCounter);
        foreach (ColorNoteVisuals cnv in _colorNoteVisuals)
        {
          // Console.WriteLine("J");
          MaterialPropertyBlockController mbpc = ReflectionUtil.GetPrivateField<MaterialPropertyBlockController>(cnv, "_materialPropertyBlockController");
          NoteController nc = ReflectionUtil.GetPrivateField<NoteController>(cnv, "_noteController");
          NoteData.NoteType nt = nc.noteData.noteType;
          if (nt == NoteData.NoteType.NoteA)
          {
            // Console.WriteLine("M");
            mbpc.materialPropertyBlock.SetColor("_Color", this._leftColor.ColorWithAlpha(.8f));
          }
          else if (nt == NoteData.NoteType.NoteB)
          {
            // Console.WriteLine("N");
            mbpc.materialPropertyBlock.SetColor("_Color", this._rightColor.ColorWithAlpha(.8f));
          }
          // Console.WriteLine("L");
        }
      }

      foreach (NoteDebris nd in _noteDebris)
      {
        // Console.WriteLine("O");
        MaterialPropertyBlockController mbpc = ReflectionUtil.GetPrivateField<MaterialPropertyBlockController>(nd, "_materialPropertyBlockController");
        MaterialPropertyBlock mbp = mbpc.materialPropertyBlock;
        Color c = mbp.GetColor("_Color"); 

        if (CheckIsUnchangedRed(c))
        {
          // Console.WriteLine("P");
          mbp.SetColor("_Color", this._leftColor.ColorWithAlpha(.8f));
        }
        else if (CheckIsUnchangedBlue(c))
        {
          // Console.WriteLine("Q");
          mbp.SetColor("_Color", this._rightColor.ColorWithAlpha(.8f));
        }
        // Console.WriteLine("R");
      }

      foreach (NoteMeshCutDebris nd in _noteMeshCutDebris)
      {
        // Console.WriteLine("S");
        MaterialPropertyBlockController mbpc = ReflectionUtil.GetPrivateField<MaterialPropertyBlockController>(nd, "_materialPropertyBlockController");
        MaterialPropertyBlock mbp = mbpc.materialPropertyBlock;
        Color c = mbp.GetColor("_CutoutEdgeColor");

        if (CheckIsUnchangedRed(c))
        {
          // Console.WriteLine("T");
          mbp.SetColor("_CutoutEdgeColor", this._leftColor.ColorWithAlpha(.8f));
        }
        else if (CheckIsUnchangedBlue(c))
        {
          // Console.WriteLine("U");
          mbp.SetColor("_CutoutEdgeColor", this._rightColor.ColorWithAlpha(.8f));
        }
        // Console.WriteLine("V");
      }

      foreach (XWeaponTrail wt in _weaponTrails)
      {
        // Console.WriteLine("W");
        Color c = ReflectionUtil.GetPrivateField<Color>(wt, "MyColor");

        if (CheckIsUnchangedRed(c))
        {
          // Console.WriteLine("X");
          ReflectionUtil.SetPrivateField(wt, "MyColor", this._leftColor.ColorWithAlpha(.8f));
        }
        else if (CheckIsUnchangedBlue(c))
        {
          // Console.WriteLine("Y");
          ReflectionUtil.SetPrivateField(wt, "MyColor", this._rightColor.ColorWithAlpha(.8f));
        }
        // Console.WriteLine("Z");
      }

      foreach (BeatEffect be in _beatEffects)
      {
        // Console.WriteLine("AA");
        SpriteRenderer[] srs = ReflectionUtil.GetPrivateField<SpriteRenderer[]>(be, "_spriteRenderers");

        foreach (SpriteRenderer sr in srs)
        {
          Color c = sr.color;
          if (CheckIsUnchangedRed(c))
          {
            // Console.WriteLine("AB");
            sr.color = this._leftColor;
          }
          else if (CheckIsUnchangedBlue(c))
          {
            // Console.WriteLine("AC");
            sr.color = this._rightColor;
          }
          // Console.WriteLine("AD");
        }
      }
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLateUpdate()
    {



    }
  }
}

