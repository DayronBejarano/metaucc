using System;

public enum Gender { Male, Female }

[Serializable]
public class AvatarData
{
    // Datos básicos
    public Gender selectedGender;
    public int skinColorIndex;
    public string hairId;

    // Blend Shapes corporales
    public float bodyFat;
    public float bodyMuscle;
    public float bodyHeight;
    public float breastSize;

    // Blend Shapes faciales
    public float noseLength;
    public float lipsWidth;
    public float jawWidth;
    public float browWidth;
    public float browHeight;
    public float eyesSize;
    public float eyesClosedDefault;

    // Índices de estilos y materiales
    public int hairMaterialIndex;
    public int facialHairStyleIndex;
    public int facialHairMaterialIndex;
    public int eyeMaterialIndex;
    public int lashesMaterialIndex;
    public int browMaterialIndex;
    public int nailsMaterialIndex;

    // Ropa completa
    public string clothingItemHatName;
    public int clothingItemHatVariation;

    public string clothingItemTopName;
    public int clothingItemTopVariation;

    public string clothingItemBottomName;
    public int clothingItemBottomVariation;

    public string clothingItemShoesName;
    public int clothingItemShoesVariation;

    public string clothingItemGlassesName;
    public int clothingItemGlassesVariation;

    public AvatarData()
    {
        selectedGender = Gender.Male;
        skinColorIndex = 0;
        hairId = "";

        bodyFat = 0f;
        bodyMuscle = 0f;
        bodyHeight = 1.8f;
        breastSize = 0f;

        noseLength = 0f;
        lipsWidth = 0f;
        jawWidth = 0f;
        browWidth = 0f;
        browHeight = 0f;
        eyesSize = 0f;
        eyesClosedDefault = 0f;

        hairMaterialIndex = 0;
        facialHairStyleIndex = 0;
        facialHairMaterialIndex = 0;
        eyeMaterialIndex = 0;
        lashesMaterialIndex = 0;
        browMaterialIndex = 0;
        nailsMaterialIndex = 0;

        clothingItemHatName = "";
        clothingItemHatVariation = 0;

        clothingItemTopName = "";
        clothingItemTopVariation = 0;

        clothingItemBottomName = "";
        clothingItemBottomVariation = 0;

        clothingItemShoesName = "";
        clothingItemShoesVariation = 0;

        clothingItemGlassesName = "";
        clothingItemGlassesVariation = 0;
    }
}
