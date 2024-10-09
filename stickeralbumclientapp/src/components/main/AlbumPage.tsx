
import Sticker from "../sticker/Sticker";

import "./AlbumPage.css"

const AlbumPage = ({page, album, stickers, onAddStickers} : {
  page: number,
  album : number[],
  stickers: number[],
  onAddStickers: (stickerIds: number[]) => void}) =>
{
  const stickersPerPage = [1, 2, 3, 4, 5];
  const getStickerId = (index: number) => (page-1)*stickersPerPage.length + index;

  const renderSticker = (index: number, album: number[]) => {
    const stickerId = getStickerId(index);
    const isFilled = album.includes(Number(stickerId));
    const isNew = !isFilled && stickers.map(s => Number(s)).includes(stickerId);
    return <div
      className={`album-sticker${isFilled ? "" : "-empty"} album-sticker-${index} ${isNew ? "album-sticker-new" : ""}`}
      onClick={() => onAddStickers([stickerId])}>
        {isFilled ? <Sticker stickerId={stickerId}/> : <span className="album-sticker-number">{stickerId}</span>}
        {isNew && <span className="stick-text">Stick!</span>}
    </div>
  }

  return <div className="album-page">
    {stickersPerPage.map(index => renderSticker(index, album))}
  </div>
}

export default AlbumPage;
