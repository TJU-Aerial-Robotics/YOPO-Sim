using System.IO;
using UnityEngine;

namespace Diablo.Utils {
    public class DataManager : MonoBehaviour {
        public string DataFolderName = "TrainingData";
        public string SceneFolderPrefix = "Scene_";
        public string TextureFolderName = "Textures";
        public string TextureFilePrefix = "depth_";
        public string tomlFileName = "data.toml";
        public string terrainFileName = "terrain";
        public string treeFileName = "tree";
        public void ClearData() {
            if (Directory.Exists(GetFullDataFolderPath())) {
                Directory.Delete(GetFullDataFolderPath(), true);
            }
        }
        public string GetFullDataFolderPath() {
            var basePath = Directory.GetParent(Application.dataPath).FullName;
            string dataPath = Path.Combine(basePath, DataFolderName);
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            return dataPath;
        }
        public string GetFullSceneFolderPath(int sceneIndex) {
            string scenePath = Path.Combine(GetFullDataFolderPath(), $"{SceneFolderPrefix}{sceneIndex}");
            if (!Directory.Exists(scenePath)) {
                Directory.CreateDirectory(scenePath);
            }
            return scenePath;
        }
        public string GetFullTextureFolderPath(int sceneIndex) {
            string texturePath = Path.Combine(GetFullSceneFolderPath(sceneIndex), TextureFolderName);
            if (!Directory.Exists(texturePath)) {
                Directory.CreateDirectory(texturePath);
            }
            return texturePath;
        }
        public string GetTextrueFileName(int textureIndex) {
            return $"{TextureFilePrefix}{textureIndex}.exr";
        }
        public string GetFullTextureFilePath(int sceneIndex, int textureIndex) {
            return Path.Combine(GetFullTextureFolderPath(sceneIndex), GetTextrueFileName(textureIndex));
        }
        public string GetFullTomlFilePath(int sceneIndex) {
            return Path.Combine(GetFullSceneFolderPath(sceneIndex), tomlFileName);
        }
        public string GetTreeFileName() {
            return treeFileName;
        }
        public string GetTerrainFileName() {
            return terrainFileName;
        }

    }
}
