using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace CheatInstaller
{
    public class Program
    {
        #region Info
        public static readonly string CheatName = "GolfWithYourCheats.dll";
        public static readonly string ExePath = "/Golf With Your Friends.exe";
        public static readonly string ManagedPath = "/Golf With Your Friends_Data/Managed";
        public static readonly string AssemblyCSharpPath = ManagedPath + "/Assembly-CSharp-firstpass.dll";
        public static readonly string HackPath = ManagedPath + "/" + CheatName;
        #endregion

        #region Variables
        private static Program _Instance;

        private string _Directory = "";
        #endregion

        #region Properties
        private string ExeLocation => _Directory + ExePath;
        private string AssemblyCSharpLocation => _Directory + AssemblyCSharpPath;
        private string HackLocation => _Directory + HackPath;
        #endregion

        static void Main(string[] args)
        {
            Console.Title = "GolfWithYourCheats by AtiLion";

            Console.WriteLine("GolfWithYourCheats - A cheat/hack for the game Golf With Your Friends.");
            Console.WriteLine("Created by AtiLion");
            Console.WriteLine();

            _Instance = new Program();
            _Instance.Run();
        }

        #region Static Functions
        public static bool IsValidDirectory(string path) =>
            Directory.Exists(path) && File.Exists(path + ExePath) && File.Exists(path + AssemblyCSharpPath);
        #endregion

        #region Functions
        private void Exit()
        {
            Console.WriteLine("Press any key to exit!");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void Run()
        {
            Console.WriteLine("Enter location to the Golf With Your Friends directory!");
            do
            {
                Console.Write("Location: ");
                _Directory = Console.ReadLine();
            } while (string.IsNullOrEmpty(_Directory) && !IsValidDirectory(_Directory));

            Console.WriteLine("Press any key to install!");
            Console.ReadKey();

            Install();
            Exit();
        }

        private void Install()
        {
            // Check for existing cheats
            Console.WriteLine("Checking for existing cheat...");
            if (File.Exists(HackLocation))
                File.Delete(HackLocation);
            if (File.Exists(AssemblyCSharpLocation + ".backup"))
                File.Delete(AssemblyCSharpLocation + ".backup");
            Console.WriteLine("Checked for existing cheat!");

            // Copy the dll
            Console.WriteLine("Copying cheat dll...");
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + "/" + CheatName, HackLocation);
                File.Copy(AssemblyCSharpLocation, AssemblyCSharpLocation + ".backup");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to copy the cheat! Please check the dll files!");
                Console.WriteLine("Error: " + ex);
                Exit();
            }
            Console.WriteLine("Copied cheat dll!");

            // Install the cheat
            Console.WriteLine("Installing cheat...");
            try
            {
                Console.WriteLine("Opening assemblies...");
                ModuleDefMD asmAssembly = ModuleDefMD.Load(File.ReadAllBytes(AssemblyCSharpLocation));
                ModuleDefMD asmHack = ModuleDefMD.Load(File.ReadAllBytes(HackLocation));
                Console.WriteLine("Opened assemblies!");

                Console.WriteLine("Getting hook function...");
                TypeDef typLoader = asmHack.GetTypes().FirstOrDefault(a => a.FullName == "CheatModule.Loader");
                if(typLoader == null)
                {
                    Console.WriteLine("Cheat loader class not found!");
                    Exit();
                }
                TypeDef typMainMenu = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "MainMenu");
                if (typMainMenu == null)
                {
                    Console.WriteLine("Game MainMenu class not found!");
                    Exit();
                }
                MethodDef mthHook = typLoader.Methods.FirstOrDefault(a => a.Name == "Hook" && a.IsPublic && a.IsStatic);
                if(mthHook == null)
                {
                    Console.WriteLine("Cheat hook method not found!");
                    Exit();
                }
                MethodDef mthStart = typMainMenu.Methods.FirstOrDefault(a => a.Name == "Start" && a.IsPrivate && !a.IsStatic);
                if(mthStart == null)
                {
                    Console.WriteLine("Game Start method not found!");
                    Exit();
                }
                Console.WriteLine("Gotten hook functions!");

                Console.WriteLine("Getting anticheat functions....");
                TypeDef typACBase = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "CodeStage.AntiCheat.Detectors.ActDetectorBase");
                if(typACBase == null)
                {
                    Console.WriteLine("Anticheat base class not found!");
                    Exit();
                }
                TypeDef typACInjection = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "CodeStage.AntiCheat.Detectors.InjectionDetector");
                if(typACInjection == null)
                {
                    Console.WriteLine("Anticheat injection class not found!");
                    Exit();
                }
                TypeDef typACObscured = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector");
                if(typACObscured == null)
                {
                    Console.WriteLine("Anticheat obscured class not found!");
                    Exit();
                }
                TypeDef typACSpeedHack = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "CodeStage.AntiCheat.Detectors.SpeedHackDetector");
                if(typACSpeedHack == null)
                {
                    Console.WriteLine("Anticheat speedhack class not found!");
                    Exit();
                }
                TypeDef typACWallHack = asmAssembly.GetTypes().FirstOrDefault(a => a.FullName == "CodeStage.AntiCheat.Detectors.WallHackDetector");
                if(typACWallHack == null)
                {
                    Console.WriteLine("Anticheat wallhack class not found!");
                    Exit();
                }
                MethodDef mthACBaseStart = typACBase.Methods.FirstOrDefault(a => a.Name == "Start" && a.IsPrivate && !a.IsStatic);
                if(mthACBaseStart == null)
                {
                    Console.WriteLine("Anticheat base start method not found!");
                    Exit();
                }
                MethodDef mthACBaseEnable = typACBase.Methods.FirstOrDefault(a => a.Name == "OnEnable" && a.IsPrivate && !a.IsStatic);
                if(mthACBaseEnable == null)
                {
                    Console.WriteLine("Anticheat base enable method not found!");
                    Exit();
                }
                MethodDef mthACInjectionAwake = typACInjection.Methods.FirstOrDefault(a => a.Name == "Awake" && a.IsPrivate && !a.IsStatic);
                if(mthACInjectionAwake == null)
                {
                    Console.WriteLine("Anticheat injection awake method not found!");
                    Exit();
                }
                MethodDef mthACObscuredAwake = typACObscured.Methods.FirstOrDefault(a => a.Name == "Awake" && a.IsPrivate && !a.IsStatic);
                if(mthACObscuredAwake == null)
                {
                    Console.WriteLine("Anticheat obscured awake method not found!");
                    Exit();
                }
                MethodDef mthACSpeedHackAwake = typACSpeedHack.Methods.FirstOrDefault(a => a.Name == "Awake" && a.IsPrivate && !a.IsStatic);
                if(mthACSpeedHackAwake == null)
                {
                    Console.WriteLine("Anticheat speedhack awake method not found!");
                    Exit();
                }
                MethodDef mthACSpeedHackUpdate = typACSpeedHack.Methods.FirstOrDefault(a => a.Name == "Update" && a.IsPrivate && !a.IsStatic);
                if(mthACSpeedHackUpdate == null)
                {
                    Console.WriteLine("Anticheat speedhack update method not found!");
                    Exit();
                }
                MethodDef mthACWallHackAwake = typACWallHack.Methods.FirstOrDefault(a => a.Name == "Awake" && a.IsPrivate && !a.IsStatic);
                if(mthACWallHackAwake == null)
                {
                    Console.WriteLine("Anticheat wallhack awake method not found!");
                    Exit();
                }
                MethodDef mthACWallHackFixedUpdate = typACWallHack.Methods.FirstOrDefault(a => a.Name == "FixedUpdate" && a.IsPrivate && !a.IsStatic);
                if(mthACWallHackFixedUpdate == null)
                {
                    Console.WriteLine("Anticheat wallhack fixedupdate method not found!");
                    Exit();
                }
                MethodDef mthACWallHackUpdate = typACWallHack.Methods.FirstOrDefault(a => a.Name == "Update" && a.IsPrivate && !a.IsStatic);
                if(mthACWallHackUpdate == null)
                {
                    Console.WriteLine("Anticheat wallhack update method not found!");
                    Exit();
                }
                Console.WriteLine("Gotten all anticheat methods!");

                Console.WriteLine("Removing anticheat...");
                mthACBaseEnable.Body.Instructions.Clear();
                mthACBaseStart.Body.Instructions.Clear();
                mthACInjectionAwake.Body.Instructions.Clear();
                mthACObscuredAwake.Body.Instructions.Clear();
                mthACSpeedHackAwake.Body.Instructions.Clear();
                mthACSpeedHackUpdate.Body.Instructions.Clear();
                mthACWallHackAwake.Body.Instructions.Clear();
                mthACWallHackFixedUpdate.Body.Instructions.Clear();
                mthACWallHackUpdate.Body.Instructions.Clear();
                Console.WriteLine("Anticheat removed!");

                Console.WriteLine("Adding hook....");
                Importer importer = new Importer(asmAssembly);
                ITypeDefOrRef typrefLoader = importer.Import(new TypeRefUser(asmHack, "GolfWithYourCheats", "Loader"));
                IMethod mthrefHook = importer.Import(mthHook);
                mthStart.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, mthrefHook));
                mthACBaseEnable.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACBaseStart.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACInjectionAwake.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACObscuredAwake.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACSpeedHackAwake.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACSpeedHackUpdate.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACWallHackAwake.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACWallHackFixedUpdate.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                mthACWallHackUpdate.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                Console.WriteLine("Hook added!");

                Console.WriteLine("Saving assembly....");
                asmAssembly.Write(AssemblyCSharpLocation);
                Console.WriteLine("Assembly written!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to install the cheat! Please check the dll files!");
                Console.WriteLine("Error: " + ex);
                Exit();
            }
            Console.WriteLine("Cheat installed!");
        }
        #endregion
    }
}
