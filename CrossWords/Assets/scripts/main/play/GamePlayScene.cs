using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScene : MonoBehaviour
{
    [SerializeField] Board board;

    readonly MoveStack _moveStack = new MoveStack();

    void Awake()
    {
        if (board == null)
            board = FindFirstObjectByType<Board>();
    }

    void Start()
    {
        WireLetterButtons();
    }

    void WireLetterButtons()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            string n = button.gameObject.name;
            if (n.Length != 4 || !n.StartsWith("But", StringComparison.Ordinal))
                continue;

            char c = n[3];
            if (c < 'A' || c > 'Z')
                continue;

            button.onClick.RemoveAllListeners();
            char letter = c;
            button.onClick.AddListener(() => OnLetterButton(button, letter));
        }
    }

    void OnLetterButton(Button button, char letter)
    {
        if (board == null || !board.TryGetSelectedCell(out int x, out int y))
            return;

        board.SetCell(x, y, letter);
        _moveStack.Add(letter, x, y);
        button.interactable = false;
    }

    void Update()
    {
        if (board != null)
            board.UpdateBoard();
    }
}
