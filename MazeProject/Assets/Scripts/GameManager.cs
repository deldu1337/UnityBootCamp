using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SliderController _sliderController;
    [SerializeField] private Button _createMazeButton;
    [SerializeField] private TMP_Text _sizeText;

    [SerializeField] private Board _board;
    [SerializeField] private Player _player;

    private void Start()
    {
        _sliderController.SliderValueChange += SetSlideValue;
        _createMazeButton.onClick.AddListener(CreateMaze);
    }

    private void SetSlideValue(float val)
    {
        _sizeText.text = val.ToString();
        _board.Size = (int)val;
    }

    private void CreateMaze()
    {
        _board.Initialze();
        _board.Spawn();

        _player.Initialze(1, 1, _board);
    }
}
