using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] options;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactionSound;
    private RectTransform rect;
    private int currentOption = 0;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(1);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }
    }

    private void ChangePosition(int _change)
    {
        currentOption += _change;
        if (changeSound != null)
        {
            //AudioManager.instance.PlayAudioClip(changeSound);
        }

        //check if the current option is out of bounds
        if (currentOption < 0)
        {
            currentOption = options.Length - 1;
        }
        else if (currentOption >= options.Length)
        {
            currentOption = 0;
        }

        //Asign Y posion of the option to the arrow move up and down
        rect.position = new Vector3(rect.position.x, options[currentOption].position.y, rect.position.z);
    }

    private void Interact()
    {
        if (interactionSound != null)
        {
            //AudioManager.instance.PlayAudioClip(interactionSound);
        }
        //Call the method of the current option
        options[currentOption].GetComponent<Button>().onClick.Invoke();
    }
}
