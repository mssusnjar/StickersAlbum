import { useState } from "react";
import { DefaultButton, PrimaryButton, Stack, StackItem, TextField } from "@fluentui/react";
import { login, register } from "../../services/AuthenticationService";

import "./LoginView.css"

const LoginView = ({getPlayerInfo} : {getPlayerInfo: (username: string) => void}) =>
{
  const [usernameValue, setUsernameValue] = useState("");
  const [passwordValue, setPasswordValue] = useState("");
  const [incorrectCredentials, setIncorrectCredentials] = useState(false);

  const onLogin = () => {
    login(usernameValue, passwordValue).then(
      (response) => {
        if (response.succeeded)
        {
          setIncorrectCredentials(false);
          getPlayerInfo(response.username);
        }
        else setIncorrectCredentials(true);
      });
  }

  const onRegister = () => {
    setIncorrectCredentials(false);
    register(usernameValue, passwordValue).then(
      (response) => {
        if (response.succeeded) getPlayerInfo(response.username);
        else setIncorrectCredentials(true);
      });
  }

  return <Stack className="login-container">
      <StackItem className="login-title">
        Login
      </StackItem>
      <Stack className="login-content">
        <TextField
          label="Username"
          value={usernameValue}
          onChange={(_, newValue) => setUsernameValue(newValue || '')}
        />
        <TextField
          label="Password"
          value={passwordValue}
          onChange={(_, newValue) => setPasswordValue(newValue || '')}
          type="password"
          canRevealPassword
          revealPasswordAriaLabel="Show password"
          errorMessage={incorrectCredentials ? "Incorrect username or password" : (passwordValue.length < 8 ? "Password must be at least 8 characters long" : "")}
        />
      </Stack>
      <PrimaryButton className="login-button" onClick={onLogin}>Login</PrimaryButton>
      <DefaultButton className="login-button" onClick={onRegister}>Register</DefaultButton>
    </Stack>
}

export default LoginView;
