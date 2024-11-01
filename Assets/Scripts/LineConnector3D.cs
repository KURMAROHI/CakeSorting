using UnityEngine;
using System.Collections.Generic;

public class LineConnector3D : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private LineRenderer linePrefab;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private LayerMask connectableLayer;
    [SerializeField] private float maxConnectionDistance = 10f;
    
    [Header("Visual Settings")]
    [SerializeField] private Color validConnectionColor = Color.green;
    [SerializeField] private Color invalidConnectionColor = Color.red;
    [SerializeField] private float lineWidth = 0.1f;
    
    private bool isDragging = false;
    private GameObject currentObject;
    private LineRenderer currentLine;
    private List<GameObject> connectedObjects = new List<GameObject>();
    private List<LineRenderer> activeLines = new List<LineRenderer>();
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartConnection();
        }
        else if (Input.GetMouseButton(0))
        {
            UpdateConnection();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndConnection();
        }
    }

    Vector3 GetMouseWorldPosition(float distanceFromCamera = 10f)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, currentObject ? currentObject.transform.position : Vector3.zero);
        
        float enter;
        if (plane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        
        return ray.GetPoint(distanceFromCamera);
    }

    void StartConnection()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, connectableLayer))
        {
            isDragging = true;
            currentObject = hit.collider.gameObject;
            connectedObjects.Clear();
            connectedObjects.Add(currentObject);

            // Create new line
            currentLine = Instantiate(linePrefab);
            currentLine.positionCount = 2;
            currentLine.startWidth = lineWidth;
            currentLine.endWidth = lineWidth;
            activeLines.Add(currentLine);
        }
    }

    void UpdateConnection()
    {
        if (!isDragging || currentLine == null) return;

        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 startPos = currentObject.transform.position;
        
        // Update line position
        currentLine.SetPosition(0, startPos);
        currentLine.SetPosition(1, mousePosition);

        // Check for new connections
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxConnectionDistance, connectableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            bool isValidConnection = IsValidConnection(hitObject);
            
            // Update line color based on valid connection
            currentLine.material.color = isValidConnection ? validConnectionColor : invalidConnectionColor;

            if (!connectedObjects.Contains(hitObject) && isValidConnection)
            {
                // Complete current line
                currentLine.SetPosition(1, hitObject.transform.position);

                // Start new line
                currentObject = hitObject;
                connectedObjects.Add(currentObject);

                currentLine = Instantiate(linePrefab);
                currentLine.positionCount = 2;
                currentLine.startWidth = lineWidth;
                currentLine.endWidth = lineWidth;
                activeLines.Add(currentLine);

                // Visual feedback
                AddConnectionEffect(hitObject.transform.position);

                // Check if we have a match (3 or more objects)
                if (connectedObjects.Count >= 3)
                {
                    CheckForMatch();
                }
            }
        }
        else
        {
            // No valid target, show red line
            currentLine.material.color = invalidConnectionColor;
        }
    }

    void EndConnection()
    {
        if (!isDragging) return;

        isDragging = false;
        
        // Remove the last incomplete line if it exists
        if (currentLine != null)
        {
            activeLines.Remove(currentLine);
            Destroy(currentLine.gameObject);
        }

        // If we don't have enough objects for a match, clear all lines
        if (connectedObjects.Count < 3)
        {
            ClearLines();
        }
        else
        {
            // Process the final match
            ProcessMatch();
        }

        currentLine = null;
        currentObject = null;
    }

    bool IsValidConnection(GameObject obj)
    {
        if (obj == currentObject) return false;

        // Distance check
        float distance = Vector3.Distance(obj.transform.position, currentObject.transform.position);
        if (distance > minDistance) return false;

        // Line of sight check
        Ray ray = new Ray(currentObject.transform.position, obj.transform.position - currentObject.transform.position);
        if (Physics.Raycast(ray, distance, ~connectableLayer)) return false;

        // Add additional validation here (same type, color, etc.)
        return true;
    }

    void CheckForMatch()
    {
        // Add your match validation logic here
        // For example, check if all connected objects are the same type/color
        bool isValidMatch = ValidateMatch();
        
        if (isValidMatch)
        {
            foreach (LineRenderer line in activeLines)
            {
                line.material.color = validConnectionColor;
            }
        }
    }

    bool ValidateMatch()
    {
        // Add your match validation logic here
        // Example: Check if all objects have the same component or tag
        if (connectedObjects.Count < 3) return false;

        string firstTag = connectedObjects[0].tag;
        return connectedObjects.TrueForAll(obj => obj.CompareTag(firstTag));
    }

    void ProcessMatch()
    {
        if (!ValidateMatch()) return;

        foreach (GameObject obj in connectedObjects)
        {
            // Add match effects
            AddMatchEffect(obj);
            
            // Add your match processing logic here
            // Example: Score points, destroy objects, spawn new ones, etc.
        }

        // Clear after processing
        ClearLines();
    }

    void AddConnectionEffect(Vector3 position)
    {
        // Add connection visual feedback
        // Example: Particle effect or temporary highlight
        GameObject connectionEffect = new GameObject("ConnectionEffect");
        connectionEffect.transform.position = position;
        // Add particle system or other visual effect
        Destroy(connectionEffect, 1f);
    }

    void AddMatchEffect(GameObject matchedObject)
    {
        // Add match visual feedback
        // Example: Particle effect, scaling animation, etc.
        GameObject matchEffect = new GameObject("MatchEffect");
        matchEffect.transform.position = matchedObject.transform.position;
        // Add particle system or other visual effect
        Destroy(matchEffect, 1f);
    }

    void ClearLines()
    {
        foreach (LineRenderer line in activeLines)
        {
            Destroy(line.gameObject);
        }
        activeLines.Clear();
        connectedObjects.Clear();
    }

    void OnDrawGizmos()
    {
        // Draw connection radius in editor
        if (currentObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentObject.transform.position, minDistance);
        }
    }
}