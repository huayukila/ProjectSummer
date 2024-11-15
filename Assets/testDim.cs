using UnityEngine;
using MPostProcess;
public class testDim : MonoBehaviour
{
  private MonochromeEffect monochrome;
  private float rateTimeCnt;
  // Start is called before the first frame update
  void Start()
  {
    rateTimeCnt = 0f;
    monochrome = new MonochromeEffect(Camera.main);
  }

  // Update is called once per frame
  void Update()
  {
    if (monochrome != null && monochrome.IsActive)
    {
      rateTimeCnt += Time.deltaTime;
      monochrome.SetRate(Mathf.Abs(Mathf.Sin(rateTimeCnt * Mathf.PI * 2f * 0.1f)));
    }
    if (Input.GetMouseButtonDown(0))
    {
      monochrome.SetActive(true);
    }

    if (Input.GetMouseButtonDown(1))
    {
      rateTimeCnt = 0f;
      monochrome.SetActive(false);
    }
  }
}
