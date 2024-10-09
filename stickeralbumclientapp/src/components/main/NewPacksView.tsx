import { PrimaryButton, Stack, StackItem, Text } from "@fluentui/react";
import Pack from "../sticker/Pack";
import NewPacksTimer from "./NewPacksTimer";

import "./NewPacksView.css"

const maxPacksCount : number = 3;

const getNextNewPacksTime = (newPacksTime: string) => {
  const dateTime = new Date(newPacksTime);

  if (dateTime < new Date())
  {
    return undefined;
  }

  return dateTime.getTime();
}

const NewPacksView = ({packsCount, newPacksTime, coins, updatePlayerInfo, onOpenPacks, onBuyPacks} : {
  packsCount : number,
  newPacksTime: string,
  coins: number,
  updatePlayerInfo: () => void,
  onOpenPacks: (packsCount: number) => void
  onBuyPacks: (packsCount: number) => void}) =>
{
  const nextNewPacksTime = getNextNewPacksTime(newPacksTime);

  return <Stack className="new-packs-container" key={packsCount}>
      <StackItem className="new-packs-container-title">
        Your packs
      </StackItem>
      <StackItem>
        {packsCount > 0 && <Text>You have {packsCount} new {packsCount > 1 ? "packs" : "pack"}</Text>}
        {packsCount === 0 && <Text>You have opened all your packs</Text>}
      </StackItem>
      <StackItem>
        {packsCount < maxPacksCount && nextNewPacksTime &&
          <div>New free packs in <NewPacksTimer targetTime={nextNewPacksTime} onZero={updatePlayerInfo} key={newPacksTime}/></div>
        }
        {packsCount === maxPacksCount &&
          <div>Open your packs to continue receiving free packs</div>
        }
      </StackItem>
      <StackItem className = "new-packs-stack">
        {packsCount > 2 && <div className="new-pack-3"><Pack /></div>}
        {packsCount > 1 && <div className="new-pack-2"><Pack /></div>}
        {packsCount > 0 &&
          <div className="new-pack-1" onClick={() => onOpenPacks(1)}>
            <Pack />
            <span className="open-pack-text">Open</span>
          </div>}
        {packsCount === 0 && <Text>No new packs here</Text>}
      </StackItem>
      <PrimaryButton className="new-packs-button" disabled={packsCount == 0} onClick={() => onOpenPacks(packsCount)}>Open all</PrimaryButton>
      <StackItem className = "buy-packs-stack">
        <div>You have {coins} coins.</div>
      </StackItem>
      <PrimaryButton className="new-packs-button" disabled={coins < 30 || packsCount === maxPacksCount} onClick={() => onBuyPacks(1)}>Buy a pack (30 coins)</PrimaryButton>
    </Stack>
}

export default NewPacksView;
