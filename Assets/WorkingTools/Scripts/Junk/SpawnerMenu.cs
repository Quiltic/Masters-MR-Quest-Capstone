//using RadialMenu;
//using RadialMenu.Contracts;
//using RadialMenu.Enums;
//using RadialMenu.MenuTypes;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class SpawnerMenu : MonoBehaviour
//{

    
//    private OVRCameraRig _cameraRig;
//    private const float SpawnDistanceFromCamera = 0.75f;
//    private IRadialMenu radMenu;
//    public IRadialMenuItem[] radialMenuItems;
//    public UnityEngine.UIElements.PanelSettings panelSettings;
//    public Sprite icon;


//    private void Awake()
//    {
//        radialMenuItems = new IRadialMenuItem[]
//            {
//                new radialOption("Upgrade to wood", "Upgrade the element to wood", icon, Color.red, Color.white),
//                new radialOption("Upgrade to stone", "Upgrade the element to stone", icon, Color.red, Color.white),
//                new radialOption("Upgrade to metal", "Upgrade the element to metal", icon, Color.red, Color.white),
//                new radialOption("Upgrade to hqm", "Upgrade the element to hqm", icon, Color.red, Color.white),
//                new radialOption("Demolish", "Demolish the element", icon, Color.red, Color.white),
//            };

//        radMenu = RadialMenuBuilder
//            .Create(panelSettings, radialMenuItems) // Define menu items
//            .WithVisibilityAnimationTime(0.1f) // Set animation time
//            .WithMainOuterRadius(236, 240) // Define outer radius
//            .WithMainInnerRadius(150, 160) // Define inner radius
//            .WithMainColors(Color.white, Color.red, Color.white, Color.red) // Set colors
//            .WithActionOnClickOutOfBounds(RadialMenuAction.PerformItemAndClose) // Define what happens when clicked out of radial menu bounds
//            .Build<ScaledRadialMenu>(); // Build the menu with 0.1s scale animation on open and close

//        radMenu.Show();
//        _cameraRig = FindAnyObjectByType<OVRCameraRig>();
//    }

    
//    void Update()
//    {
//        if (OVRInput.GetDown(OVRInput.RawButton.Start))
//        {
//            ToggleMenu(!gameObject.activeInHierarchy);
//        }
//    }


//    private void ToggleMenu(bool active)
//    {
//        gameObject.SetActive(active);
//        if (active)
//        {
//            StartCoroutine(SnapCanvasInFrontOfCamera());
//        }
//    }

//    private IEnumerator SnapCanvasInFrontOfCamera()
//    {
//        yield return new WaitUntil(
//            () => _cameraRig && _cameraRig.centerEyeAnchor.transform.position != Vector3.zero);
//        transform.position = _cameraRig.centerEyeAnchor.transform.position +
//                             _cameraRig.centerEyeAnchor.transform.forward * SpawnDistanceFromCamera;
//    }


//    public class UpgradeMenuCenterElement : RadialMenuItemCenterElement
//    {
//        public UpgradeMenuCenterElement(Sprite icon, Color iconColor, string name, string description)
//        {
//            style.alignItems = Align.Center;
//            style.justifyContent = Justify.Center;

//            Add(new Image()
//            {
//                sprite = icon,
//                tintColor = iconColor
//            });
//            Add(new Label()
//            {
//                text = name
//            });
//            Add(new Label()
//            {
//                text = description
//            });
//        }
//    }

//    public class UpgradeMenuElement : RadialMenuItemElement
//    {
//        private readonly Color _regularColor;
//        private readonly Color _highlightedColor;
//        private Image _image;

//        public UpgradeMenuElement(Sprite icon, Color regularColor, Color highlightedColor)
//        {
//            Add(_image = new Image()
//            {
//                name = "element-icon"
//            });

//            _regularColor = regularColor;
//            _highlightedColor = highlightedColor;

//            _image.style.backgroundImage = new StyleBackground(icon);
//            _image.style.unityBackgroundImageTintColor = regularColor;
//        }

//        protected override void OnHighlightStarted()
//        {
//            _image.style.unityBackgroundImageTintColor = _highlightedColor;
//        }

//        protected override void OnHighlightEnded()
//        {
//            _image.style.unityBackgroundImageTintColor = _regularColor;
//        }
//    }

//    public record radialOption : IRadialMenuItem
//    {
//        public string Name { get; private set; }
//        public string Description { get; private set; }
//        public Sprite Icon { get; private set; }

//        private readonly Color _regularColor;
//        private readonly Color _highlightedColor;

//        public radialOption(string upgradeName, string description, Sprite icon, Color regularColor, Color highlightedColor)
//        {
//            Name = upgradeName;
//            Description = description;
//            Icon = icon;

//            _regularColor = regularColor;
//            _highlightedColor = highlightedColor;
//        }

//        public IRadialMenuItemElement CreateItemElement() => new UpgradeMenuElement(Icon, _regularColor, _highlightedColor);

//        public IRadialMenuItemCenterElement CreateItemCenterElement() => new UpgradeMenuCenterElement(Icon, _regularColor, Name, Description);

//        public void OnItemPerform()
//        {
//            Debug.Log(Name + " performed");
//        }
//    }
//}
