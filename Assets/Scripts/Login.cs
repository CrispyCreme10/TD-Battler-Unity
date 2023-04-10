using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{
    private VisualElement _root;

    private TextField _username;
    private TextField _password;
    private TextField _passwordMatch;

    private Button _login;
    private Button _signup;
    
    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _username = _root.Q<TextField>("Username");
        _password = _root.Q<TextField>("Password");
        _passwordMatch = _root.Q<TextField>("MatchPassword");

        _login = _root.Q<Button>("Login");
        _signup = _root.Q<Button>("Signup");
    }
}
