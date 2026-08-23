using UnityEngine;
using DG.Tweening;                                                                                                                                                                                                                     

public class highlightTutorial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerMovementFixed player;
    [SerializeField] private GameObject arrowSprite;
    [SerializeField] private GameObject arrowSprite2;

    private int step = 0;
    void Start()
    {
        arrowSprite.SetActive(true);
        arrowSprite2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (step == 0 && InventorySystem.Instance.GetActiveItem() != null)
        {
            AdvanceTo(1);
        }
        else if (step == 1 && InventorySystem.Instance.GetActiveItem() == null && WasThrown())
        {
            AdvanceTo(2);
        }
    }

    void AdvanceTo(int next)
    {
        step = next;

        if (step == 1)
        {
            arrowSprite.SetActive(false);
            arrowSprite2.SetActive(true);
            arrowSprite2.transform.DOLocalMoveY(arrowSprite2.transform.localPosition.y + 1.5f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
        }

        if (step == 2)
        {
            arrowSprite2.SetActive(false);
        }
    }
    private bool wasThrown = false;

    bool WasThrown()
    {
        bool thrown = wasThrown && InventorySystem.Instance.GetActiveItem() == null;
        wasThrown = InventorySystem.Instance.GetActiveItem() != null;
        return thrown;
    }
}
