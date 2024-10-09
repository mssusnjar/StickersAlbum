
import { Stack, StackItem } from "@fluentui/react";
import { ChevronLeft48Filled, ChevronRight48Regular } from "@fluentui/react-icons";
import CoverFront from "../../resources/coverFront.png"
import CoverBack from "../../resources/CoverBack.png"
import AlbumPage from "./AlbumPage";

import "./AlbumView.css"

const AlbumView = ({album, page, setPage, stickers, onAddStickers} : {
  album : number[],
  page: number,
  setPage: (newPage: number) => void,
  stickers: number[],
  onAddStickers: (stickerIds: number[]) => void}) =>
{
  const maxPages = 16;
  const coverPage : boolean = (page < 1) || (page > maxPages);

  return <Stack className="album-view-container" >
    <StackItem className="album-view-container-title">
        Your album
    </StackItem>
    <Stack className="album-view-inner-container" horizontal>
      <ChevronLeft48Filled className={`album-arrow-button${page<1 ? "disabled" : ""}`} onClick={() => setPage(page > 1 ? page - 2 : 0)}/>
        {!coverPage ?
        <Stack className="album-container" horizontal>
          <Stack>
            <AlbumPage album={album} page={page-1} stickers={stickers} onAddStickers={onAddStickers}/>
          </Stack>
          <Stack>
            <AlbumPage album={album} page={page} stickers={stickers} onAddStickers={onAddStickers}/>
          </Stack>
        </Stack>
        :
        <div className="album-cover-container">
          <img className={`album-cover-${page === 0 ? "front" : "back"}`} src={page === 0 ? CoverFront : CoverBack} />
        </div>}
      <ChevronRight48Regular className={`album-arrow-button${page > maxPages ? "disabled" : ""}`} onClick={() => setPage(page <= maxPages ? page + 2 : page)}/>
    </Stack>
    </Stack>
}

export default AlbumView;