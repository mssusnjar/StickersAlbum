import { Stack, StackItem } from "@fluentui/react";
import { TradeOffer } from "../../services/TradingService";
import TradeOfferElement from "./TradeOfferElement";

import "./TradingList.css"

const TradingList = ({title, buttonText, trades, onButtonClick, buttonDisabled} : {
  title: string,
  buttonText: string,
  trades: TradeOffer[],
  onButtonClick: (trade: TradeOffer) => void
  buttonDisabled: (trade: TradeOffer) => boolean}) =>
{

  return <Stack className="trading-list-container">
    <StackItem className="trading-list-container-title">
      {title}
    </StackItem>
    <Stack className="trading-list-content">
      {trades.length > 0 ?
        trades.map(element => <TradeOfferElement trade={element} buttonText={buttonText} onButtonClick={onButtonClick} buttonDisabled={buttonDisabled(element)}/>) :
        "No trades to display"
      }
    </Stack>
  </Stack>
}

export default TradingList;
