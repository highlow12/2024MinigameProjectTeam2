using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Linq;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance;
    private Command[] commands = new Command[3];
    private Image consoleImage;
    private bool isHost = false;
    public bool isFocused = false;
    public TMP_InputField inputField;
    public TMP_Text consoleText;
    public string currentCommand;

    public struct Command
    {
        public string name;
        public string description;
        public string usage;
        public List<string> parameters;
    }

    // Commands

    private Command help = new()
    {
        name = "help",
        description = "Display all available commands",
        usage = "help",
        parameters = new List<string>()
    };

    private Command panel = new()
    {
        name = "panel",
        description = "Toggle the debug panel",
        usage = "panel",
        parameters = new List<string>()
    };

    private Command modify = new()
    {
        name = "modify",
        description = "Modify the game state",
        usage = "modify <parameter> <value>",
        parameters = new List<string>()
    };

    private Command skill = new()
    {
        name = "skill",
        description = "Execute the boss skill",
        usage = "skill <skill>",
        parameters = new List<string>()
    };


    void Awake()
    {
        Instance = this;
        consoleImage = GetComponent<Image>();
        consoleImage.enabled = false;
        // add commands
        commands[0] = help;
        commands[1] = panel;
        commands[2] = modify;
        commands[3] = skill;
    }

    void Start()
    {
        // isHost = NetworkRunner.Instances.First().IsServer;
        isHost = true;
    }

    void Update()
    {
        if (!isHost)
        {
            return;
        }
        // execute command
        if (Input.GetKeyDown(KeyCode.Return) && isFocused && currentCommand != "")
        {
            CommandParser();
            AddLine(currentCommand);
            inputField.text = "";
        }
    }

    public void ToggleFocus()
    {
        isFocused = !isFocused;
        if (isFocused)
        {
            inputField.ActivateInputField();
            consoleImage.enabled = true;
        }
        else
        {
            inputField.DeactivateInputField();
            consoleImage.enabled = false;
        }
    }

    public void ChangeCommand()
    {
        currentCommand = inputField.text;
    }

    void AddLine(string line)
    {
        // add line to console
        consoleText.text += line + "\n";
    }

    void CommandParser()
    {
        // parse command
        List<string> splitCommand = currentCommand.Split(' ').ToList();
        // get command
        Command command = commands.FirstOrDefault(c => c.name == splitCommand[0]);
        if (command.name == null)
        {
            AddLine("Invalid command");
            return;
        }
        switch (command.name)
        {
            case "help":
                // HelpCommand();
                break;
            case "panel":
                // PanelCommand();
                break;
            case "modify":
                ModifyCommand(splitCommand.GetRange(1, splitCommand.Count - 1));
                break;
        }

    }

    // Command functions
    void ModifyCommand(List<string> parameters)
    {
        // modify boss state
        BossMonsterNetworked boss = GameObject.FindWithTag("Boss").GetComponent<BossMonsterNetworked>();
        if (parameters.Count < 2)
        {
            Debug.Log("Invalid parameters");
            return;
        }
        string parameter = parameters[0];
        string value = parameters[1];
        switch (parameter)
        {
            case "phase":
                // modify boss phase
                boss.BossPhase = int.Parse(value);
                break;
            case "health":
                // modify boss health
                boss.CurrentHealth = int.Parse(value);
                break;
            default:
                Debug.Log("Invalid parameter");
                break;
        }
    }

}
