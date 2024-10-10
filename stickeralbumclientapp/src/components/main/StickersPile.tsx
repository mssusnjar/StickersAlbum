import { PrimaryButton, Stack, StackItem } from "@fluentui/react";
import Sticker, { getStickerPrice } from "../sticker/Sticker";

import "./StickerPile.css"

const StickerInPile = ({stickerId, count, isNew, onHoverText, onClick, multipleButtons, onSecondaryClick} : {
  stickerId : number,
  count: number | undefined,
  isNew: boolean,
  onHoverText: string,
  onClick: () => void,
  multipleButtons: boolean,
  onSecondaryClick: () => void}) =>
{
  if (!count) return;

  return <Stack>
    <div className="sticker-pile-element" onClick={onClick}>
      <Sticker stickerId={stickerId} />
      {count > 1 && <div className="number-circle">{count > 9 ? "9+" : count}</div>}
      {isNew && <span className="new-circle">New</span>}
      {(multipleButtons || isNew) && <span className="new-instruction-text">{onHoverText}</span>}
    </div>
    {multipleButtons && <PrimaryButton className="sticker-pile-element-button" onClick={onSecondaryClick}>Sell ({getStickerPrice(stickerId)} coins)</PrimaryButton>}
    </Stack>
}

const StickersPile = ({stickerMap, album, onHoverText, onClick, singleButton, onSecondaryClick} : {
  stickerMap : Map<number, number>,
  album: number[],
  onHoverText: string,
  onClick: (stickerIds: number[]) => void,
  singleButton: boolean,
  onSecondaryClick: (stickerId: number, coins: number) => void}) =>
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
        multipleButtons={!singleButton}
        onSecondaryClick={() => onSecondaryClick(stickerId, getStickerPrice(stickerId))}
      />)}
    </Stack>
    {singleButton && <PrimaryButton
      className="stickers-pile-button"
      onClick={() => onClick(stickers.filter((_, index) => newStatus[index]))}
      disabled={!newStatus.includes(true)}>
        Stick all
    </PrimaryButton>}
  </Stack>
}

export default StickersPile;
