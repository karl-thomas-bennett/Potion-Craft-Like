using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public float speed = 0.1f;
    bool doMove = false;
    private GameObject backgroundObjects;
    private void Start()
    {
        backgroundObjects = GameObject.Find("BackgroundObjects");
    }
    public void Move(bool move)
    {
        doMove = move;
    }
    private void Update()
    {
        if (doMove)
        {
            if(transform.childCount == 0)
            {
                return;
            }
            Line line = transform.GetChild(0).GetComponent<Line>();
            if(line.points.Count < 2)
            {
                return;
            }
            float distanceToNextPoint = (line.points[1] - line.points[0]).magnitude;
            
            float distanceToTravel = speed*Time.deltaTime;
            int index = 1;
            while(distanceToNextPoint < distanceToTravel)
            {
                distanceToTravel -= distanceToNextPoint;
                if(index + 1 == line.points.Count)
                {
                    distanceToTravel = distanceToNextPoint;
                    break;
                }
                distanceToNextPoint = (line.points[index + 1] - line.points[index]).magnitude;
                index++;
            }
            Vector2 direction = (line.points[index] - line.points[0]).normalized;
            for (int j = index - 1; j > 0; j--)
            {
                line.points.RemoveAt(0);
            }

            for (int i = 0; i < line.points.Count; i++)
            {
                line.points[i] -= direction*distanceToTravel;
                
            }
            backgroundObjects.transform.position -= (Vector3)(direction * (distanceToTravel * line.transform.lossyScale));
            if (line.points.Count < 2 || (line.points[1].x == 0 && line.points[1].y == 0))
            {
                Destroy(line.gameObject);
                return;
            }
            line.points[0] = Vector2.zero;
            line.Reload();
        }
    }
    public void AddLine(Line line)
    {
        if(transform.childCount == 0)
        {
            Line l = Instantiate(line, transform.position + (Vector3)line.points[0], Quaternion.identity, transform);
            l.name = "Line";
            l.Reload();
        }
        else
        {
            //Merge lines
            List<Vector2> points = new List<Vector2>
            {
                new Vector2(0, 0)
            };
            Line oldLine = transform.GetChild(0).GetComponent<Line>();
            float[] oldDistances = oldLine.GetDistances();
            float[] newDistances = line.GetDistances();
            int newIndex = 0;
            float oldDistance = 0;
            float newDistance = newDistances[0];
            //Merge new line and old line until we run out of the new line, then add the rest of the old line to the end
            for(int oldIndex = 0; oldIndex < oldDistances.Length; oldIndex++)
            {
                if (newIndex == newDistances.Length - 1)
                {
                    Vector2 point = oldLine.points[oldIndex + 1] - oldLine.points[oldIndex];
                    points.Add(points[points.Count - 1] + point);
                    continue;
                }
                oldDistance = oldDistances[oldIndex];
                if(oldDistance == newDistance)
                {
                    Vector2 point1 = (oldLine.points[oldIndex + 1] - oldLine.points[oldIndex]).normalized * newDistance;
                    Vector2 point2 = line.points[newIndex + 1] - line.points[newIndex];
                    Vector2 newPoint = point1 + point2;
                    if (newPoint.magnitude > 0.01f)
                    {
                        points.Add(points[points.Count - 1] + newPoint);
                    }
                    newIndex++;
                    newDistance = newDistances[newIndex];
                    continue;
                }
                while(oldDistance > newDistance)
                {
                    Vector2 point1 = (oldLine.points[oldIndex + 1] - oldLine.points[oldIndex]).normalized * newDistance;
                    Vector2 point2 = line.points[newIndex + 1] - line.points[newIndex];
                    Vector2 newPoint = point1 + point2;
                    if(newPoint.magnitude > 0.01f)
                    {
                        points.Add(points[points.Count - 1] + newPoint);
                    }
                    
                    oldDistance -= newDistance;
                    newIndex++;
                    if(newIndex == newDistances.Length - 1)
                    {
                        newDistance = 0;
                        break;
                    }
                    newDistance = newDistances[newIndex];
                }
                if(newDistance > oldDistance)
                {
                    Vector2 point1 = (line.points[newIndex + 1] - line.points[newIndex]).normalized * oldDistance;
                    Vector2 point2 = oldLine.points[oldIndex + 1] - oldLine.points[oldIndex];
                    Vector2 newPoint = point1 + point2;
                    if (newPoint.magnitude > 0.01f)
                    {
                        points.Add(points[points.Count - 1] + newPoint);
                    }
                    newDistance -= oldDistance;
                }else if (newIndex == newDistances.Length - 1 && oldDistance != 0)
                {
                    Vector2 point = (oldLine.points[oldIndex + 1] - oldLine.points[oldIndex]).normalized;
                    points.Add(points[points.Count - 1] + point*oldDistance);
                }
            }
            //if we run out of the old line instead, add the rest of the new line to the end
            if(newIndex != newDistances.Length - 1 || newDistance != 0)
            {
                if(newDistance != 0)
                {
                    Vector2 point = (line.points[newIndex + 1] - line.points[newIndex]).normalized;
                    points.Add(points[points.Count - 1] + point * newDistance);
                    newIndex++;
                }
                while (newIndex < newDistances.Length)
                {
                    Vector2 point = line.points[newIndex + 1] - line.points[newIndex];
                    points.Add(points[points.Count - 1] + point);
                    newIndex++;
                }
            }
            oldLine.points = points;
            oldLine.Reload();
        }
        
    }
}
