using UnityEngine;

public class AfterImageGenerator : MonoBehaviour
{
    
    
    

    [TextArea(10, 13)]
    

    public GameObject afterImagePrefab;

    private AfterImageMaterials _AfterImageMaterials;
    private SpriteRenderer _SpriteRenderer;
    public AfterImage[] _AfterImages { get; private set; }

    /// <summary>
    /// How many AfterImage classes and sprites should CreateAfterImages() instantiate? 
    /// If you want to add more than 6, make sure you have the same amount of materials in the materials
    /// array located in the class <see cref="AfterImageMaterials"/>.
    /// </summary>
    private const int MAX_AMOUNT = 6;

    private void Awake()
    {
        _AfterImageMaterials = FindObjectOfType<AfterImageMaterials>();
        TryGetComponent(out _SpriteRenderer);
        CreateAfterImages();
    }

    private void CreateAfterImages()
    {
        if (_SpriteRenderer == null || _AfterImageMaterials == null || afterImagePrefab == null)
        {
            Debug.LogError(gameObject.name + ": Has no SpriteRenderer or no AfterImagePrefab or AfterImageMaterials was not found!");
            Debug.Break();
            return;
        }

        // Note: I think AfterImageGroup could be further optimized by instantiating a prefab instead of adding the component. 
        // Create a new gameobject and add a AfterImageGroup class to it.
        AfterImageGroup group = new GameObject(gameObject.name + "_AfterImage_Group").AddComponent<AfterImageGroup>();

        // Initialize _AfterImages array.
        _AfterImages = new AfterImage[MAX_AMOUNT];

        // Populate the array.
        for (int i = 0; i < MAX_AMOUNT; i++)
        {
            // Instantiate afterImagePrefab and store a AfterImage class variable at the same time.
            AfterImage afterImage = Instantiate(afterImagePrefab, _AfterImageMaterials.transform).GetComponent<AfterImage>();

            // Name it to have everything organized in the scene hierarchy.
            afterImage.name = gameObject.name + "_AfterImage" + i;

            // Make it a child of the group transform (again to organize).
            afterImage.transform.SetParent(group.transform);

            // Set the properties that should be used by this AfterImage.
            afterImage.SetProperties(_SpriteRenderer.sprite, _AfterImageMaterials.materials[i], posAndRotReference: transform);

            // Add it to the _AfterImages array.
            _AfterImages[i] = afterImage;
        }

        // Pass the _AfterImages array to the group array.
        group.AfterImages = _AfterImages;
    }
}
