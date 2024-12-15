using UnityEngine;

public class MapStatue : MonoBehaviour
{
    public Transform player; // 실제 플레이어
    public RectTransform smallMap; // 작은 지도의 RectTransform
    public Vector2 mapSize; // 작은 지도의 크기 (Inspector에서 설정)
    public Vector2 worldSize; // 실제 미로의 크기 (Inspector에서 설정)

    private Vector2 scale; // 축소 비율

    private void Start()
    {
        // 축소 비율 계산 (지도의 크기를 실제 월드 크기로 나눔)
        scale = new Vector2(
            mapSize.x / worldSize.x,
            mapSize.y / worldSize.y
        );
    }

    private void Update()
    {
        // 실제 플레이어의 월드 좌표 가져오기
        Vector3 playerWorldPos = player.position;

        // 작은 지도에서의 위치 계산
        Vector2 playerMapPos = new Vector2(
            playerWorldPos.x * scale.x,
            playerWorldPos.y * scale.y
        );

        // 작은 지도의 중심을 기준으로 위치 조정
        playerMapPos -= mapSize / 2f;

        // 현재 오브젝트(SmallPlayer)의 위치 업데이트
        GetComponent<RectTransform>().anchoredPosition = playerMapPos;
    }
}
