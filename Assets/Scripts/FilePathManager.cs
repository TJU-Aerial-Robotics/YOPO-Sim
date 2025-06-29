using System.IO;
using UnityEngine;

namespace YOPO.SIM {
    public class FilePathManager : MonoBehaviour {
        [SerializeField] private string dataFolderName = "TrainingData";
        [SerializeField] public string sceneFolderPrefix = "Scene_";
        [SerializeField] private string imageFolderName = "Textures";
        [SerializeField] private string tomlFileName = "data.toml";
        [SerializeField] private string terrainFileName = "terrain";
        [SerializeField] private string treeFileName = "tree";

        private void CreateFolderIfNotExists(string folderPath) {
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
        }
        private void DeleteFolderIfExists(string folderPath) {
            if (Directory.Exists(folderPath)) {
                Directory.Delete(folderPath, true);
            }
        }

        public void DeleteDataFolder() {
            DeleteFolderIfExists(GetDataFolderPath());
        }

        private string GetDataFolderPath() {
            var basePath = Directory.GetParent(Application.dataPath).FullName;
            string dataPath = Path.Combine(basePath, dataFolderName);
            CreateFolderIfNotExists(dataPath);
            return dataPath;
        }

        public string GetSceneFolderPath(int sceneIndex) {
            string scenePath = Path.Combine(GetDataFolderPath(), $"{sceneFolderPrefix}{sceneIndex}");
            CreateFolderIfNotExists(scenePath);
            return scenePath;
        }
        public string GetTextureFolderPath(int sceneIndex) {
            string texturePath = Path.Combine(GetSceneFolderPath(sceneIndex), imageFolderName);
            CreateFolderIfNotExists(texturePath);
            return texturePath;
        }
        public string GetImageFilePath(int sceneIndex, string imageFileName) {
            return Path.Combine(GetTextureFolderPath(sceneIndex), imageFileName);
        }
        public string GetTomlFilePath(int sceneIndex) {
            return Path.Combine(GetSceneFolderPath(sceneIndex), tomlFileName);
        }
        public string GetTreeFileName() {
            return treeFileName;
        }
        public string GetTerrainFileName() {
            return terrainFileName;
        }

        public void SetDataFolderName(string dataFolderName) {
            this.dataFolderName = dataFolderName;
        }
    }
}