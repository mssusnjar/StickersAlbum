
import StickersList from "./StickersList";
import "./Sticker.css"

export const getStickerPrice = (stickerId: number) => stickerId % 5 === 4 ? 10 : 5;

const Sticker = ({stickerId} : {stickerId : number}) => {
  const isGold = stickerId % 5 === 4;
  return (<div className={`sticker-${isGold ? "gold" : "regular"}`}>
      <img src={StickersList[stickerId - 1]} width="128" height="128"/>
    </div>);
}

export default Sticker;