using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance;
    private List<Line> lines = new();
    public List<string> commandHistory = new();
    private Command[] commands = new Command[5];
    private Image consoleImage;
    private bool isHost = false;
    private Coroutine hideLogCoroutine;
    private Command currentCommand;
    private NetworkRunner _runner;
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
        // this string will be displayed in console when command is executed successfully
        public string successMessage;
        public List<string> parameters;
        // these properties are used for tool tip
        public string description;
        public string usage;
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
        usage = "modify <state> <value>",
        successMessage = "Game state: {parameter1} modified to {parameter2}",
        parameters = new List<string>(),
        availableParameters = new List<string> { "phase", "health" }
    };

    private Command skill = new()
    {
        name = "skill",
        description = "Execute the boss skill",
        successMessage = "Boss skill: {parameter1} executed",
        usage = "skill <skillName>",
        parameters = new List<string>(),
        availableParameters = new List<string>()
    };

    private Command changeClass = new()
    {
        name = "changeClass",
        description = "Change the player class",
        successMessage = "Player {parameter1} class changed to {parameter2}",
        usage = "changeClass <playerRef> <classId>",
        parameters = new List<string>(),
        availableParameters = new List<string>(),
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
        commands[4] = changeClass;
    }

    void Start()
    {
        _runner = NetworkRunner.Instances.First();
        isHost = _runner.IsServer;
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
            // Up arrow, Down arrow command history
            CommandHistory();
            if (currentCommandText != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    commandHistory.Add(currentCommandText);
                    bool commandResult = ExecuteCommand(currentCommand);
                    if (commandResult)
                    {
                        string successMessage = currentCommand.successMessage;
                        // replace parameters in success message
                        int idx = 1;
                        foreach (string parameter in currentCommand.parameters)
                        {
                            successMessage = successMessage.Replace($"{{parameter{idx}}}", parameter);
                            idx++;
                        }
                        AddLine(successMessage);
                    }
                    else
                    {
                        AddLine("Command failed", LineType.Error);
                    }
                    inputField.text = "";
                    historyCursor = commandHistory.Count;
                    RenderLines();
                }
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
        // remove backslashes from input for avoid the argument exception on regex match
        string newText = inputField.text.Replace("\\", "");
        currentCommandText = newText;
        requireParse = true;
    }


    IEnumerator HideLog()
    {
        // hide log after 3 seconds
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
            // white
            case LineType.Info:
                newLine = $"<b><color=#FFFFFF>{line}</color></b>";
                break;
            // yellow
            case LineType.Warning:
                newLine = $"<color=#FFFF00>{line}</color>";
                break;
            // red
            case LineType.Error:
                newLine = $"<b><color=#FF0000>{line}</color></b>";
                break;
        }
        lines.Add(new Line
        {
            text = newLine,
            messageType = MessageType.Local,
            tick = (int)_runner.Tick
        });
    }

    void RenderLines()
    {
        // render all lines
        consoleText.text = "";
        // sort lines by tick
        lines.Sort((a, b) => a.tick.CompareTo(b.tick));
        foreach (Line line in lines)
        {
            consoleText.text += line.text + "\n";
        }
    }

    void CommandHistory()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (historyCursor == 0)
            {
                AddLine("No more command history", LineType.Warning);
            }
            else if (historyCursor >= 0)
            {
                if (historyCursor != 0)
                {
                    historyCursor--;
                }
                inputField.text = commandHistory[historyCursor];
                inputField.caretPosition = inputField.text.Length;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (historyCursor == commandHistory.Count - 1)
            {
                AddLine("No command history", LineType.Warning);
            }
            else if (historyCursor < commandHistory.Count - 1)
            {
                historyCursor++;
                inputField.text = commandHistory[historyCursor];
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }

    Command ParseCommand()
    {
        // split text to get command and parameters
        List<string> splitCommand = currentCommandText.Split(' ').ToList();
        // get toolTip command with regex
        // get command that is not default and matches with currentCommandText
        Command toolTipCommand = commands.FirstOrDefault(c => !Equals(c, default(Command)) && Regex.Match(c.name, splitCommand[0], RegexOptions.IgnoreCase).Success);
        // get executable command with exact match
        Command command = commands.FirstOrDefault(c => c.name == splitCommand[0]);
        // get parameters
        command.parameters = splitCommand.GetRange(1, splitCommand.Count - 1);
        // tool tip logic
        string displayToolTipText = "";
        if (toolTipCommand.name != null)
        {
            // display tool tip
            toolTipObject.SetActive(true);
            // if no parameters given, display command description
            if (splitCommand.Count == 1)
            {
                displayToolTipText = $"{toolTipCommand.name}: {toolTipCommand.description}\nUsage: {toolTipCommand.usage}";
            }
            // if parameters given and command has available parameters, display available parameters
            else if (toolTipCommand.availableParameters != null)
            {
                // get specific boss skills by phase
                if (toolTipCommand.name == "skill" && toolTipCommand.availableParameters.Count == 0)
                {
                    int phase = GameObject.FindWithTag("Boss").GetComponent<BossMonsterNetworked>().BossPhase;
                    List<string> parameters = AttackManager.Instance.BossAttacks
                        .Where(attack => attack.phase == phase).
                            Select(attack => attack.name)
                                .ToList();
                    toolTipCommand.availableParameters = parameters;
                    commands[3].availableParameters = parameters;
                }
                if (toolTipCommand.name == "changeClass")
                {
                    if (splitCommand.Count == 2)
                    {
                        // playerRef parameter
                        List<string> parameters = new();
                        foreach (PlayerRef playerRef in _runner.ActivePlayers)
                        {
                            parameters.Add(playerRef.PlayerId.ToString());
                        }
                        toolTipCommand.availableParameters = parameters;

                    }
                    if (splitCommand.Count == 3)
                    {
                        // classId parameter
                        List<string> parameters = new();
                        for (int i = 0; i < Enum.GetValues(typeof(CharacterClassEnum)).Length; i++)
                        {
                            parameters.Add(i.ToString());
                        }
                        toolTipCommand.availableParameters = parameters;
                    }
                }
                // get matched parameters with regex
                List<string> matchedParameters = new();
                foreach (string parameter in toolTipCommand.availableParameters)
                {
                    Match match = Regex.Match(parameter, splitCommand.Last(), RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        matchedParameters.Add(parameter);
                    }
                }
                displayToolTipText = $"\nAvailable parameters: {string.Join(", ", matchedParameters)}";
            }
            toolTipText.text = displayToolTipText;
        }
        else
        {
            toolTipObject.SetActive(false);
            toolTipText.text = displayToolTipText;
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
            case "changeClass":
                result = ChangeClassCommand(command.parameters);
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
        string text = "";
        text += "Available commands:\n";
        text += "===================================\n";
        foreach (Command command in commands)
        {
            text += $"{command.name}: {command.description}\nUsage: {command.usage}\n";
        }
        text += "===================================";
        AddLine(text);
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
                    // clear available parameters for skill command
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
        bool result;
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

    bool ChangeClassCommand(List<string> parameters)
    {
        try
        {
            // change player class
            PlayerRef[] activePlayers = _runner.ActivePlayers.ToArray();
            int.TryParse(parameters[0], out int i_playerRef);
            int.TryParse(parameters[1], out int i_classId);
            PlayerRef currentPlayerRef = new();
            foreach (PlayerRef playerRef in activePlayers)
            {
                if (playerRef.PlayerId == i_playerRef)
                {
                    currentPlayerRef = playerRef;
                }
            }
            if (Equals(currentPlayerRef, default(PlayerRef)))
            {
                AddLine("Player not found", LineType.Error);
                return false;
            }
            if (i_classId > Enum.GetValues(typeof(CharacterClassEnum)).Length - 1)
            {
                AddLine("Invalid class id", LineType.Error);
                return false;
            }
            _runner.TryGetPlayerObject(currentPlayerRef, out NetworkObject playerObject);
            playerObject.GetComponent<PlayerControllerNetworked>().RPC_SetClass(i_classId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
