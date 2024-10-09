import { useEffect, useState } from "react";
import { Stack, StackItem } from "@fluentui/react";
import StickersPile from "../main/StickersPile";
import { UserInfoResponse } from "../../services/AuthenticationService";
import TradingList from "./TradingList";
import { TradeOffer, cancelTrade, completeTrade, createTrade, getActiveTrades, getRecentTradesForUser, markTrades } from "../../services/TradingService";
import TradingDialog from "./TradingDialog";

import "./TradingContent.css"

const TradingContent = ({userInfo, updatePlayerInfo} : {userInfo : UserInfoResponse, updatePlayerInfo: () => void}) => {

  const loaderIntervalInMs = 10000;

  const [trades, setTrades] = useState<TradeOffer[]>([]);
  const [completedTrades, setCompletedTrades] = useState<TradeOffer[]>([]);
  const [dialogStickerId, setDialogStickerId] = useState<number | undefined>(undefined);

  const load = async (username : string) => {

    let activeTrades = await getActiveTrades();
    let recentUserTrades = username ? await getRecentTradesForUser(username) : [];

    if (activeTrades) {
      setTrades([...activeTrades.sort((a,b) => new Date(a.dateCreated).getTime() - new Date(b.dateCreated).getTime())]);
    }

    if (recentUserTrades && recentUserTrades.length > 0) {
      await markTrades(username, recentUserTrades.map(t => t.id));
      completedTrades.unshift(...recentUserTrades);
      setCompletedTrades([...completedTrades]);
    }
  }

  const refreshView = async () => {
    updatePlayerInfo();
    await load(userInfo.username);
  }

  useEffect(() => {
    load(userInfo.username);
    let loader = setInterval(() => load(userInfo.username), loaderIntervalInMs);

    return () => { if (loader) clearInterval(loader) };
  }, []);

  const onCompleteTrade = async (trade: TradeOffer) => {
    await completeTrade(trade.id, trade.playerId, userInfo.username);
    await refreshView();
  }

  const onCancelTrade = async (trade: TradeOffer) => {
    await cancelTrade(trade.id, userInfo.username);
    await refreshView();
  }

  const onCreateTrade = async (wantedStickerId: number, coins: number) => {
    await createTrade(userInfo.username, dialogStickerId, wantedStickerId, coins);
    setDialogStickerId(undefined);
    await refreshView();
  }

  const tradeDisabled = (trade: TradeOffer) => {
    if (trade.coins > userInfo.coins) return true;
    if (!Array.from(userInfo.stickers.keys()).map(x => Number(x)).includes(trade.wantedStickerId)) return true;
    return false;
  }

  return <>
    <Stack className="trading-content">
      <Stack className="trading-content-top" horizontal>
          <TradingList title="All trades" buttonText="Trade" trades={trades.filter(t => t.playerId !== userInfo.username)} onButtonClick={onCompleteTrade} buttonDisabled={tradeDisabled}/>
          <TradingList title="Your offers" buttonText="Cancel" trades={trades.filter(t => t.playerId === userInfo.username)} onButtonClick={onCancelTrade} buttonDisabled={(_) => false }/>
          <TradingList title="Recently completed" buttonText="" trades={completedTrades} onButtonClick={() => {}} buttonDisabled={(_) => false }/>
      </Stack>
      <StackItem>
        <StickersPile
          stickerMap={userInfo.stickers}
          album={userInfo.album}
          onHoverText="Trade"
          onClick={(stickerIds: number[]) => setDialogStickerId(stickerIds[0])}
          displayButton={false}
        />
      </StackItem>
    </Stack>
    {dialogStickerId && <TradingDialog
      stickerId={dialogStickerId}
      album={userInfo.album}
      onCreateTrade={onCreateTrade}
      clearDialog={() => setDialogStickerId(undefined)}
      maxCoins={userInfo.coins}
    />}
  </>
}

export default TradingContent;
