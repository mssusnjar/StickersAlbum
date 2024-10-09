import { PrimaryButton, Stack, StackItem } from "@fluentui/react";
import Sticker from "../sticker/Sticker";

import "./StickerPile.css"

const StickerInPile = ({stickerId, count, isNew, onHoverText, onClick, alwaysDisplayText} : {
  stickerId : number,
  count: number | undefined,
  isNew: boolean,
  onHoverText: string,
  alwaysDisplayText: boolean
  onClick: () => void}) =>
{
  if (!count) return;

  return <div className="sticker-pile-element" onClick={onClick}>
    <Sticker stickerId={stickerId} />
    {count > 1 && <div className="number-circle">{count > 9 ? "9+" : count}</div>}
    {isNew && <span className="new-circle">New</span>}
    {(alwaysDisplayText || isNew) && <span className="new-instruction-text">{onHoverText}</span>}
  </div>
}

const StickersPile = ({stickerMap, album, onHoverText, onClick, displayButton} : {
  stickerMap : Map<number, number>,
  album: number[],
  onHoverText: string,
  onClick: (stickerIds: number[]) => void,
  displayButton: boolean}) =>
{
  if (!stickerMap) return;

  const stickers = Array.from(stickerMap.keys());
  const newStatus = stickers.map(s => !album.includes(Number(s)));

  return <Stack className="stickers-pile-container">
    <StackItem className="stickers-pile-container-title">
        Your stickers
    </StackItem>
    <Stack className="stickers-pile" horizontal>
      {stickers.map((stickerId, index) => <StickerInPile
        key={stickerId}
        stickerId={stickerId}
        count={stickerMap.get(stickerId)}
        isNew={newStatus[index]}
        onHoverText={onHoverText}
        onClick={() => onClick([stickerId])}
        alwaysDisplayText={!displayButton}
      />)}
    </Stack>
    {displayButton && <PrimaryButton
      className="stickers-pile-button"
      onClick={() => onClick(stickers.filter((_, index) => newStatus[index]))}
      disabled={!newStatus.includes(true)}>
        Stick all
    </PrimaryButton>}
  </Stack>
}

export default StickersPile;
