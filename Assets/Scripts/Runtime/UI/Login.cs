using UnityEngine;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{
    private VisualElement _root;

    private TextField _username;
    private TextField _password;
    private TextField _passwordMatch;

    private Button _loginBtn;
    private Button _signupBtn;
    private Button _completeBtn;
    private Button _cancelBtn;
    
    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _username = _root.Q<TextField>("Username");
        _password = _root.Q<TextField>("Password");
        _passwordMatch = _root.Q<TextField>("MatchPassword");

        _loginBtn = _root.Q<Button>("Login");
        _signupBtn = _root.Q<Button>("Signup");
        _completeBtn = _root.Q<Button>("Complete");
        _cancelBtn = _root.Q<Button>("Cancel");
        
        _loginBtn.RegisterCallback<ClickEvent>(HandleLogin);
        _signupBtn.RegisterCallback<ClickEvent>(HandleSignup);
        _completeBtn.RegisterCallback<ClickEvent>(HandleSignupComplete);
        _cancelBtn.RegisterCallback<ClickEvent>(HandleSignupCancel);
    }

    private void HandleLogin(ClickEvent evt)
    {
        // validate username/email
        // validate password
    }
    
    private void HandleSignup(ClickEvent evt)
    {
        // switch to signup buttons
        ToggleSignupButtons(true);

    }
    
    private void HandleSignupComplete(ClickEvent evt)
    {
        // validate username/email
        // validate passwords
    }
    
    private void HandleSignupCancel(ClickEvent evt)
    {
        // clear text fields
        _username.value = "";
        _password.value = "";
        _passwordMatch.value = "";
        
        // switch to login buttons
        ToggleSignupButtons(false);
    }

    private void ToggleSignupButtons(bool activate)
    {
        _passwordMatch.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
        
        _loginBtn.style.display = activate ? DisplayStyle.None : DisplayStyle.Flex;
        _signupBtn.style.display = activate ? DisplayStyle.None : DisplayStyle.Flex;

        _completeBtn.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
        _cancelBtn.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
