using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Linq;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public sealed class TouchInputTester : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] GameObject _markerPrefab = null;

    Label _uiLabel;
    GameObject[] _markers;

    void PopulateMarkers(int count)
      => _markers = Enumerable.Range(0, count)
           .Select(i => Instantiate(_markerPrefab)).ToArray();

    Vector3 TransformPosition(Vector2 p)
      => new Vector3((p.x - Screen.width / 2) / Screen.height,
                     p.y / Screen.height - 0.5f, 0);

    void Start()
    {
        Application.targetFrameRate = 120;

        _uiLabel = _ui.rootVisualElement.Q<Label>("info-text");
    }

    void Update()
    {
        if (Touchscreen.current == null) return;

        var touches = Touchscreen.current.touches;

        if (_markers == null) PopulateMarkers(touches.Count);

        var text = "";

        for (var i = 0; i < touches.Count; i++)
        {
            var (touch, marker) = (touches[i], _markers[i]);

            var active = touch.phase.value is not TouchPhase.None and
                                              not TouchPhase.Ended and
                                              not TouchPhase.Canceled;
            marker.SetActive(active);
            if (!active) continue;

            marker.transform.position = TransformPosition(touch.position.value);

            text += $"{touch.touchId.value} {touch.position.value}\n";
        }

        _uiLabel.text = text;
    }
}
