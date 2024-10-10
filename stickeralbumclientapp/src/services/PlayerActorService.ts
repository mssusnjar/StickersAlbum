import { UserInfoResponse } from "./AuthenticationService";
import urlJoin from "url-join";

const baseUrl = "player";

export async function getUserInfo(username: string): Promise<UserInfoResponse> {

    let response = await fetch(urlJoin(baseUrl, `${username}/userinfo`), {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
        }
    });

    let userInfo = await response.json() as UserInfoResponse;
    userInfo.stickers = new Map(Object.entries(userInfo.stickers) as any as Map<number, number>);

    return userInfo;
}

export async function openPacks(username: string, count: number): Promise<Response> {

  let response = await fetch(urlJoin(baseUrl, `${username}/open?count=${count}`), {
      method: "PUT",
      headers: {
          'Content-Type': 'application/json',
      }
  });

  return response;
}

export async function buyPacks(username: string, count: number): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, `${username}/buy?count=${count}`), {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
        }
    });

    return response;
}

export async function addStickersToAlbum(username: string, stickerIds: number[]): Promise<Response> {

  let response = await fetch(urlJoin(baseUrl, `${username}/add`), {
      method: "PUT",
      headers: {
          'Content-Type': 'application/json',
      },
      body: JSON.stringify({StickerIds: stickerIds})
  });

  return response;
}

export async function sellSticker(username: string, stickerId: number, coins: number): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, `${username}/sell`), {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({StickerId: stickerId, Coins: coins})
    });

    return response;
}