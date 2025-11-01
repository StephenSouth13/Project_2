using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Thời gian (tính bằng giây) mà GameObject này sẽ tồn tại trước khi bị hủy.
    [Tooltip("Thời gian tồn tại của VFX (theo giây)")]
    public float destroyTime = 0.5f;

    void Start()
    {
        // Hàm Destroy sẽ hủy GameObject này sau khoảng thời gian được chỉ định.
        // Bạn cần điều chỉnh giá trị 'destroyTime' để nó khớp với độ dài của animation VFX.
        Destroy(gameObject, destroyTime);
    }
}