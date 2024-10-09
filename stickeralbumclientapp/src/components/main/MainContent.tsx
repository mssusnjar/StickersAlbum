import { useEffect, useState } from "react";
import { Stack, StackItem } from "@fluentui/react";
import { isLoggedIn, UserInfoResponse } from "../../services/AuthenticationService";
import NewPacksView from "./NewPacksView";
import AlbumView from "./AlbumView";
import StickersPile from "./StickersPile";
import LoginView from "./LoginView";

const MainContent = ({userInfo, getPlayerInfo, onOpenPacks, onAddStickers, onBuyPacks} : {
  userInfo : UserInfoResponse,
  getPlayerInfo: (username: string) => void,
  onOpenPacks: (packsCount: number) => void,
  onBuyPacks: (packsCount: number) => void,
  onAddStickers: (stickerIds: number[]) => void}) =>
{
  const [albumPage, setAlbumPage] = useState<number>(0);

  useEffect(() => {
    getPlayerInfo(userInfo.username);
  }, []);

  const getAlbumPageForSticker = (stickerId: number) => {
    return (Math.floor((stickerId-1) / 10) + 1) * 2;
  }

  const onStickerInPileClick = (stickerIds: number[]) => {
    if (stickerIds.length > 0)
    {
      setAlbumPage(getAlbumPageForSticker(stickerIds[0]));
    }

    onAddStickers(stickerIds);
  }

  return <Stack className="main-content">
    <StackItem>
      <Stack horizontal>
        {isLoggedIn(userInfo) ?
          <NewPacksView
            packsCount={userInfo.newPacksCount}
            newPacksTime={userInfo.newPacksDateTime}
            coins={userInfo.coins}
            updatePlayerInfo={() => getPlayerInfo(userInfo.username)}
            onOpenPacks={onOpenPacks}
            onBuyPacks={onBuyPacks} />
            :
          <LoginView getPlayerInfo={getPlayerInfo}/>
        }
        <AlbumView
          album={userInfo.album}
          page={albumPage}
          setPage={setAlbumPage}
          stickers={Array.from(userInfo.stickers.keys())}
          onAddStickers={onAddStickers} />
      </Stack>
    </StackItem>
    <StackItem>
      <StickersPile
        stickerMap={userInfo.stickers}
        album={userInfo.album}
        onHoverText="Stick!"
        onClick={onStickerInPileClick}
        displayButton={true} />
    </StackItem>
  </Stack>
}

export default MainContent;
