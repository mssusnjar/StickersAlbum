import { useState } from 'react';
import { Pivot, PivotItem } from "@fluentui/react";
import MainContent from '../main/MainContent';
import TradingContent from '../trading/TradingContent';
import { defaultUserInfo, UserInfoResponse } from '../../services/AuthenticationService';
import { addStickersToAlbum, buyPacks, getUserInfo, openPacks } from '../../services/PlayerActorService';

import './App.css'

const App = () => {
  const [userInfo, setUserInfo] = useState<UserInfoResponse>(defaultUserInfo);

  const getPlayerInfo = (username: string) => {
    if (!username) return;
    getUserInfo(username).then((userInfo : UserInfoResponse) => {setUserInfo({...userInfo})})
  }

  const openPlayerPacks = (packsCount: number) => {
    if (!userInfo.username) return;
    openPacks(userInfo.username, packsCount).then(() => getPlayerInfo(userInfo.username));
  }

  const buyPlayerPacks = (packsCount: number) => {
    if (!userInfo.username) return;
    buyPacks(userInfo.username, packsCount).then(() => getPlayerInfo(userInfo.username));;
  }

  const addPlayerStickers = (stickerIds: number[]) => {
    if (!userInfo.username) return;
    addStickersToAlbum(userInfo.username, stickerIds).then(() => getPlayerInfo(userInfo.username));
  }

  return <>
    <Pivot className="pivot-header" styles={{link: {width: "49.5%"}}}>
      <PivotItem headerText="Album">
        <MainContent
          userInfo={userInfo}
          getPlayerInfo={getPlayerInfo}
          onOpenPacks={openPlayerPacks}
          onBuyPacks={buyPlayerPacks}
          onAddStickers={addPlayerStickers}
        />
      </PivotItem>
      <PivotItem headerText="Trading Area">
        <TradingContent
          userInfo={userInfo}
          updatePlayerInfo={() => getPlayerInfo(userInfo.username)}
        />
      </PivotItem>
    </Pivot>
  </>
}

export default App;
