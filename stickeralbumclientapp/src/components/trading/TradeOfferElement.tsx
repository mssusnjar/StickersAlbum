/// <reference types="vite-plugin-svgr/client" />
import { PrimaryButton, Stack } from "@fluentui/react";
import { TradeOffer } from "../../services/TradingService";
import { CoinMultiple48Filled } from "@fluentui/react-icons";
import TransferSvg from "../../resources/transfer.svg?react";
import Sticker from "../sticker/Sticker";

import "./TradeOfferElement.css"

const TradeCoinsElement = ({coins} : {coins: number}) => {
  return <Stack horizontal className="trade-offer-coins">+{coins}<CoinMultiple48Filled /></Stack>
}

const TradeOfferElement = ({trade, buttonText, onButtonClick, buttonDisabled} : {
  trade: TradeOffer,
  buttonText: string,
  onButtonClick: (trade: TradeOffer) => void,
  buttonDisabled: boolean}) =>
{

  return <Stack className="trade-offer-element-container" horizontal>
    <Stack className="trade-offer-element" horizontal>
      <Sticker stickerId={trade.offeredStickerId} />
      {trade.coins > 0 && <TradeCoinsElement coins={trade.coins}/>}
      <TransferSvg />
      <Sticker stickerId={trade.wantedStickerId} />
      {trade.coins < 0 && <TradeCoinsElement coins={-trade.coins}/>}
    </Stack>
    {buttonText && <PrimaryButton className="trade-offer-button" onClick={() => onButtonClick(trade)} disabled={buttonDisabled}>{buttonText}</PrimaryButton>}
  </Stack>
}

export default TradeOfferElement;
