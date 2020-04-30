using ProgramableController.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableController.Bridges {
    public interface ModBridge {
        //string jsCall(string s);
        CpuBridge CreateCpuBridge(CpuController cpu);
        void OnLoad();
        
    }

    public interface CpuBridge {
        void SafeAwake();
        void Update();
        void FixedUpdate();
        void OnSimulateStart();
        void OnSimulateStop();
        void KeyEmulationUpdate();
        bool IsFInished();
    }
}
