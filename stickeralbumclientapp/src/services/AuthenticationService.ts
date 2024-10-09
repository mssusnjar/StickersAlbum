import urlJoin from "url-join";

const baseUrl = "auth";

export interface UserInfoResponse
{
  username: string,
  coins: number,
  newPacksCount: number,
  newPacksDateTime: string
  album: number[],
  stickers: Map<number, number>
}

export const defaultUserInfo : UserInfoResponse =
{
  username: "",
  coins: 0,
  newPacksCount: 0,
  newPacksDateTime: "",
  album: [],
  stickers: new Map()
}

export interface LoginResponse
{
  username: string,
  succeeded: boolean
}

export const isLoggedIn = (user: UserInfoResponse) => {
  return user.username !== "";
}

export async function login(username: string, password: string): Promise<LoginResponse> {

  let response = await fetch(urlJoin(baseUrl, "login"), {
      method: "PUT",
      headers: {
          'Content-Type': 'application/json',
      },
      body: JSON.stringify({ Username: username, Password: password })
  });

  let responseContent = await response.json() as boolean;
  let loginResponse: LoginResponse = { username: "", succeeded: false };

  if (responseContent)
  {
    loginResponse.username = username;
    loginResponse.succeeded = true;
  }

  return loginResponse;
}

export async function register(username: string, password: string): Promise<LoginResponse> {

  let response = await fetch(urlJoin(baseUrl, "register"), {
      method: "POST",
      headers: {
          'Content-Type': 'application/json',
      },
      body: JSON.stringify({ Username: username, Password: password })
  });

  let responseContent = await response.json() as boolean;
  let loginResponse: LoginResponse = { username: "", succeeded: false };

  if (responseContent)
  {
    loginResponse.username = username;
    loginResponse.succeeded = true;
  }

  return loginResponse;
}