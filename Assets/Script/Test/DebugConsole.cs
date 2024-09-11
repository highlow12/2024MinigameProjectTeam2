using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Linq;
using System.Text.RegularExpressions;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance;
    private List<Line> lines = new();
    private Command[] commands = new Command[4];
    private Image consoleImage;
    private bool isHost = false;
    private Coroutine hideLogCoroutine;
    private Command currentCommand;
    public int historyCursor = -1;
    public bool isFocused = false;
    public bool requireParse = false;
    public TMP_InputField inputField;
    public TMP_Text consoleText;
    public GameObject toolTipObject;
    public TMP_Text toolTipText;
    public GameObject debugPanel;
    public string currentCommandText;

    public enum MessageType
    {
        Local,
        Shared
    }

    enum LineType
    {
        Info,
        Warning,
        Error
    }

    public struct Line
    {
        public string text;
        public MessageType messageType;
        public int tick;

    }

    public struct Command
    {
        public string name;
        public string description;
        public string successMessage;
        public string usage;
        public List<string> parameters;
        public List<string> availableParameters;
    }

    // Commands

    private Command help = new()
    {
        name = "help",
        description = "Display all available commands",
        usage = "help",
        successMessage = "That's all the commands",
        parameters = new List<string>()
    };

    private Command panel = new()
    {
        name = "panel",
        description = "Toggle the debug panel",
        usage = "panel",
        successMessage = "Debug panel toggled",
        parameters = new List<string>()
    };

    private Command modify = new()
    {
        name = "modify",
        description = "Modify the game state",
        usage = "modify <parameter> <value>",
        successMessage = "Game state: {parameter} modified to {value}",
        parameters = new List<string>(),
        availableParameters = new List<string> { "phase", "health" }
    };

    private Command skill = new()
    {
        name = "skill",
        description = "Execute the boss skill",
        successMessage = "Boss skill: {parameter} executed",
        usage = "skill <parameter>",
        parameters = new List<string>(),
        availableParameters = new List<string>()
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

        if (isFocused)
        {
            RenderLines();
            if (requireParse)
            {
                currentCommand = ParseCommand();
                requireParse = false;
            }
            if (currentCommandText != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    bool commandResult = ExecuteCommand(currentCommand);
                    if (commandResult)
                    {
                        string successMessage = currentCommand.successMessage;
                        if (currentCommand.parameters.Count > 1)
                        {
                            successMessage = successMessage.Replace("{parameter}", currentCommand.parameters[0]);
                            successMessage = successMessage.Replace("{value}", currentCommand.parameters[1]);
                        }
                        else if (currentCommand.parameters.Count > 0)
                        {
                            successMessage = successMessage.Replace("{parameter}", currentCommand.parameters[0]);
                        }
                        AddLine(successMessage);
                    }
                    else
                    {
                        AddLine("Command failed", LineType.Error);
                    }
                    inputField.text = "";
                    RenderLines();
                }
                // Work in progress
                // if (Input.GetKeyDown(KeyCode.UpArrow))
                // {
                //     if (historyCursor == -1)
                //     {

                //         historyCursor = lines.Count - 1;
                //     }
                //     else if (historyCursor > 0)
                //     {
                //         historyCursor--;
                //     }
                //     inputField.text = lines[historyCursor].text;
                // }
            }
            else
            {
                toolTipObject.SetActive(false);
                toolTipText.text = "";
            }
        }
        else
        {
            inputField.DeactivateInputField();
        }

        // enable console by pressing return 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleFocus();
        }
    }

    public void ToggleFocus()
    {
        isFocused = !isFocused;
        if (isFocused)
        {
            inputField.ActivateInputField();
            consoleImage.enabled = true;
            consoleText.enabled = true;
        }
        else
        {
            inputField.DeactivateInputField();
            consoleImage.enabled = false;
            toolTipObject.SetActive(false);
            toolTipText.text = "";
            // hide console after 3 seconds
            if (hideLogCoroutine != null)
            {
                StopCoroutine(hideLogCoroutine);
            }
            hideLogCoroutine = StartCoroutine(HideLog());
        }
    }

    // This function is called by Event Listener
    public void ChangeCommand()
    {
        currentCommandText = inputField.text;
        requireParse = true;
    }


    IEnumerator HideLog()
    {
        yield return new WaitForSeconds(3);
        if (isFocused)
        {
            yield break;
        }
        consoleText.enabled = false;
    }

    void AddLine(string line, LineType lineType = LineType.Info)
    {
        // add line to console
        string newLine = "";
        switch (lineType)
        {
            case LineType.Info:
                newLine = $"<b><color=#FFFFFF>{line}</color></b>";
                break;
            case LineType.Warning:
                newLine = $"<color=#FFFF00>{line}</color>";
                break;
            case LineType.Error:
                newLine = $"<b><color=#FF0000>{line}</color></b>";
                break;
        }
        lines.Add(new Line { text = newLine, messageType = MessageType.Local, tick = (int)NetworkRunner.Instances.First().Tick });
        // consoleText.text += newLine + "\n";
    }

    void RenderLines()
    {
        // render all lines
        consoleText.text = "";
        lines.Sort((a, b) => a.tick.CompareTo(b.tick));
        foreach (Line line in lines)
        {
            consoleText.text += line.text + "\n";
        }
    }

    Command ParseCommand()
    {
        // parse command
        List<string> splitCommand = currentCommandText.Split(' ').ToList();
        // get command
        Command toolTipCommand = commands.FirstOrDefault(c => !Equals(c, default(Command)) && Regex.Match(c.name, splitCommand[0], RegexOptions.IgnoreCase).Success);
        Command command = commands.FirstOrDefault(c => c.name == splitCommand[0]);
        string text = "";
        command.parameters = splitCommand.GetRange(1, splitCommand.Count - 1);
        if (toolTipCommand.name != null)
        {
            toolTipObject.SetActive(true);
            if (splitCommand.Count == 1)
            {
                text = $"{toolTipCommand.name}: {toolTipCommand.description}\nUsage: {toolTipCommand.usage}";
            }
            else if (toolTipCommand.availableParameters != null)
            {
                if (toolTipCommand.name == "skill" && toolTipCommand.availableParameters.Count == 0)
                {
                    int phase = GameObject.FindWithTag("Boss").GetComponent<BossMonsterNetworked>().BossPhase;
                    List<string> parameters = AttackManager.Instance.BossAttacks
                        .Where(attack => attack.phase == phase).
                            Select(attack => attack.name)
                                .ToList();
                    Debug.Log(parameters);
                    toolTipCommand.availableParameters = parameters;
                    commands[3].availableParameters = parameters;
                }
                List<string> matchedParameters = new();
                foreach (string parameter in toolTipCommand.availableParameters)
                {
                    Match match = Regex.Match(parameter, splitCommand.Last(), RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        matchedParameters.Add(parameter);
                    }
                }
                text = $"\nAvailable parameters: {string.Join(", ", matchedParameters)}";
            }
            toolTipText.text = text;
        }
        else
        {
            toolTipObject.SetActive(false);
            toolTipText.text = text;
        }
        return command;

    }

    bool ExecuteCommand(Command command)
    {
        bool result = false;
        switch (command.name)
        {
            case "help":
                result = HelpCommand();
                break;
            case "panel":
                result = PanelCommand();
                break;
            case "modify":
                result = ModifyCommand(command.parameters);
                break;
            case "skill":
                result = SkillCommand(command.parameters);
                break;
            default:
                AddLine("Command not found", LineType.Error);
                break;
        }
        return result;
    }

    // Command functions
    bool HelpCommand()
    {
        // display all available commands
        AddLine("Available commands:");
        AddLine("===================================");
        foreach (Command command in commands)
        {
            AddLine($"{command.name}: {command.description}\nUsage: {command.usage}\n");
        }
        AddLine("===================================");
        return true;
    }

    bool PanelCommand()
    {
        try
        {
            // toggle debug panel
            debugPanel.SetActive(!debugPanel.activeSelf);
            return true;
        }
        catch
        {
            return false;
        }
    }

    bool ModifyCommand(List<string> parameters)
    {
        try
        {
            // modify boss state
            BossMonsterNetworked boss = GameObject.FindWithTag("Boss").GetComponent<BossMonsterNetworked>();
            if (parameters.Count < 2)
            {
                AddLine("Invalid parameters", LineType.Error);
                return false;
            }
            string parameter = parameters[0];
            string value = parameters[1];
            switch (parameter)
            {
                case "phase":
                    // modify boss phase
                    boss.BossPhase = int.Parse(value);
                    commands[3].availableParameters.Clear();
                    break;
                case "health":
                    // modify boss health
                    boss.CurrentHealth = int.Parse(value);
                    break;
                default:
                    AddLine("Invalid parameter", LineType.Error);
                    break;
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    bool SkillCommand(List<string> parameters)
    {
        bool result = false;
        try
        {
            // execute boss skill
            BossMonsterNetworked boss = GameObject.FindWithTag("Boss").GetComponent<BossMonsterNetworked>();
            if (parameters.Count < 1)
            {
                AddLine("Invalid parameters", LineType.Error);
                return false;
            }
            string parameter = parameters[0];
            result = boss.ExecuteSkill(parameter);
            return result;
        }
        catch
        {
            return false;
        }
    }

}
