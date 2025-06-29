using System.Collections.Generic;

namespace YOPO.SIM {
    class TomlData {
        public float depthCameraFarClipPlane;
        public float depthCameraHorizontalFOV;
        public List<Data> dataArray;
    }

    class Data {
        public List<string> imageFileNameList;
        public List<float> posStart;
        public float yawStart;
    }
}