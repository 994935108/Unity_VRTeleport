using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLaser : MonoBehaviour
{
	public Transform player;
	public Transform mark;
	public float rayLength = 5;
	private LineRenderer lineRenderer;
	private Vector3[] linePoints;
	private Vector3 targetPoint;
	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		linePoints = new Vector3[3];
	}
	private void Update()
	{
		

			Vector2 rotOffset = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))*Time.deltaTime*200;

			transform.localRotation *= Quaternion.Euler(new Vector3(-rotOffset.y, 0, 0));

			player.localRotation *= Quaternion.Euler(new Vector3(0, rotOffset.x, 0));
		


		if (Input.GetKey(KeyCode.Space))
		{
			Ray ray = new Ray(transform.position, transform.forward);

			RaycastHit hit;
			linePoints[0] = transform.position;


			if (Physics.Raycast(ray, out hit, rayLength))
			{

				linePoints[2] = hit.point;
				linePoints[1] = transform.position + (Vector3.ProjectOnPlane(transform.forward, Vector3.up)) * (linePoints[2] - linePoints[0]).magnitude;
				targetPoint = linePoints[2] + Vector3.up * player.position.y;
			}
			else
			{
				ray = new Ray(transform.position + transform.forward * rayLength, Vector3.down);
				if (Physics.Raycast(ray, out hit, rayLength + Mathf.Abs(transform.position.y)))
				{
					linePoints[2] = hit.point;
					linePoints[1] = transform.position + transform.forward * ((linePoints[2] - linePoints[0]).magnitude);
					targetPoint = linePoints[2] + Vector3.up * player.position.y;
				}
				else
				{
					linePoints[2] = ray.origin + ray.direction * rayLength;
					Vector3 middlePoint = transform.position + transform.forward * ((linePoints[2] - linePoints[0]).magnitude);
					linePoints[1] = middlePoint;
				}
			}
			mark.position = linePoints[2];
			mark.rotation = Quaternion.Euler(Vector3.zero);

			DrawLineRenderCurve(lineRenderer, linePoints);
		}
		else
		{
			if (Input.GetKeyUp(KeyCode.Space))
			{
				player.position = targetPoint;
				lineRenderer.positionCount = 0;
				mark.position = linePoints[2] * 1000;
			}
		}
	}

	//绘制曲线
	private void DrawLineRenderCurve(LineRenderer lineRenderer, Vector3[] points)
	{

		for (int i = 1; i <= 50; i++)
		{
			float t = i / (float)50;

			Vector3 pixel = CalculateBezierPoint(t, points[0], points[1], points[2]);
			lineRenderer.positionCount = i;
			if (i == 1)
			{
				lineRenderer.SetPosition(i - 1, points[0]);

			}
			else
			{
				lineRenderer.SetPosition(i - 1, pixel);

			}
		}
	}

	/// <summary>
	/// 计算二阶贝塞尔曲线的点 公式 （1-t）*(1-t)*p0+2t*(1-t)*p1+t*t*p2   t属于[0,1]
	/// </summary>
	/// <param name="t">  0<t<1 </param>
	/// <param name="p0">起点</param>
	/// <param name="p1">中点</param>
	/// <param name="p2">结束点</param>
	/// <returns></returns>
	internal static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		return uu * p0 + 2 * u * t * p1 + tt * p2;
	}
}
