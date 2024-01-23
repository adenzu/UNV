using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNV.Path2D;

public class UserController : MonoBehaviour
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _boatPrefab;
    [SerializeField] private GameObject _boatPathWaypointPrefab;
    [SerializeField] private Transform _boatPathParent;
    [SerializeField] private Transform _boatSelectionIndicator;

    private Transform _selectedBoatTransform;
    private Transform _cameraTransform;
    private Vector3 _previousMousePosition;

    private PathCreator _currentPathCreator;

    private void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUI();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            _cameraTransform.position -= Vector3.up * Input.mouseScrollDelta.y * 10f;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - _previousMousePosition;
            mouseDelta = new Vector3(mouseDelta.x / Screen.width, 0, mouseDelta.y / Screen.height);
            _cameraTransform.position -= mouseDelta * _cameraTransform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnBoat();
            UpdatePathObjects();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DespawnBoat();
            UpdatePathObjects();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                StartBoats();
            }
            else if (_selectedBoatTransform != null)
            {
                StartBoat(_selectedBoatTransform.GetComponent<BoatControllerPathCreator>());
            }
        }

        SelectBoat();

        EditPath();

        _previousMousePosition = Input.mousePosition;
    }

    private void ToggleUI()
    {
        _ui.SetActive(!_ui.activeSelf);
    }

    private void SpawnBoat()
    {
        GameObject newBoat = Instantiate(_boatPrefab, MouseWorldPoint(), Quaternion.identity, transform);
        _currentPathCreator = newBoat.GetComponent<PathCreator>();
        _selectedBoatTransform = newBoat.transform;
        IndicateBoatSelection();
    }

    private void DespawnBoat()
    {
        _boatSelectionIndicator.SetParent(null);
        _boatSelectionIndicator.gameObject.SetActive(false);
        if (_selectedBoatTransform == null)
        {
            return;
        }
        if (transform.childCount > 1)
        {
            Transform child = transform.GetChild(0);
            if (child == _selectedBoatTransform)                        // Destroy function is slow
            {
                child = transform.GetChild(1);
            }
            Destroy(_selectedBoatTransform.gameObject);
            _currentPathCreator = child.GetComponent<PathCreator>();
            _selectedBoatTransform = child;
        }
        else if (transform.childCount == 1)
        {
            Destroy(transform.GetChild(0).gameObject);
            _currentPathCreator = null;
            _selectedBoatTransform = null;
        }
        IndicateBoatSelection();
    }

    private void IndicateBoatSelection()
    {
        if (_selectedBoatTransform != null)
        {
            _boatSelectionIndicator.SetParent(_selectedBoatTransform);
            _boatSelectionIndicator.localPosition = Vector3.zero;
            _boatSelectionIndicator.localRotation = Quaternion.identity;
            _boatSelectionIndicator.gameObject.SetActive(true);
        }
        else
        {
            _boatSelectionIndicator.gameObject.SetActive(false);
        }
    }

    private void SelectBoat()
    {
        KeyCode[] numberKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

        for (int i = 0; i < numberKeys.Length; i++)
        {
            if (Input.GetKeyDown(numberKeys[i]))
            {
                if (i < transform.childCount)
                {
                    _selectedBoatTransform = transform.GetChild(i);
                    _currentPathCreator = _selectedBoatTransform.GetComponent<PathCreator>();
                    UpdatePathObjects();
                    _boatSelectionIndicator.SetParent(_selectedBoatTransform);
                    _boatSelectionIndicator.localPosition = Vector3.zero;
                    _boatSelectionIndicator.gameObject.SetActive(true);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _currentPathCreator = null;
            _boatPathParent.gameObject.SetActive(false);
            _boatSelectionIndicator.SetParent(null);
            _boatSelectionIndicator.gameObject.SetActive(false);
        }
    }

    private void EditPath()
    {
        if (_currentPathCreator == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 spawnPosition = _currentPathCreator.AddWaypoint(MouseWorldPoint());
            if (_boatPathParent.childCount < _currentPathCreator.Waypoints.Count)
            {
                Instantiate(_boatPathWaypointPrefab, spawnPosition, Quaternion.identity, _boatPathParent);
            }
            else
            {
                Transform child = _boatPathParent.GetChild(_currentPathCreator.Waypoints.Count - 1);
                child.gameObject.SetActive(true);
                child.position = spawnPosition;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _boatPathParent.GetChild(_currentPathCreator.Waypoints.Count - 1).gameObject.SetActive(false);
            _currentPathCreator.RemoveWaypoint();
        }
    }

    private void UpdatePathObjects()
    {
        if (_currentPathCreator != null)
        {
            _boatPathParent.gameObject.SetActive(true);
            for (int i = 0; i < _currentPathCreator.Waypoints.Count; i++)
            {
                if (i >= _boatPathParent.childCount)
                {
                    Instantiate(_boatPathWaypointPrefab, _currentPathCreator.Waypoints[i], Quaternion.identity, _boatPathParent);
                }
                else
                {
                    Transform child = _boatPathParent.GetChild(i);
                    child.position = _currentPathCreator.Waypoints[i];
                    child.gameObject.SetActive(true);
                }
            }
            for (int i = _currentPathCreator.Waypoints.Count; i < _boatPathParent.childCount; i++)
            {
                _boatPathParent.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            _boatPathParent.gameObject.SetActive(false);
        }
    }

    private void StartBoat(BoatControllerPathCreator boatControllerPathCreator)
    {
        boatControllerPathCreator.StartPath();
    }

    private void StartBoats()
    {
        foreach (Transform boat in transform)
        {
            StartBoat(boat.GetComponent<BoatControllerPathCreator>());
        }
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    private Vector3 MouseWorldPoint()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.y = 0;
        return worldPosition;
    }
}
