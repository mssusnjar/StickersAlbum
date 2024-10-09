import { useState } from "react";
import { PrimaryButton, Stack, Dialog, SwatchColorPicker, DialogType, DefaultButton, DialogFooter, SpinButton } from "@fluentui/react";

import "./TradingDialog.css"

const TradingDialog = ({stickerId, album, onCreateTrade, clearDialog, maxCoins} : {
  stickerId: number,
  album: number[],
  onCreateTrade: (wantedStickerId: number, coins: number) => void,
  clearDialog: () => void,
  maxCoins: number}) =>
{
  const [selectedId, setSelectedId] = useState<string>();
  const [coinsValue, setCoinsValue] = useState<number>(0);

  const colorCells = Array.from(Array(80), (_, index) => {
    let id = index + 1;
    return { id: id.toString(), label: `Sticker ${id}`, color: album.includes(id) ? '#0e700e' : '#fde300', disabled: id === stickerId };
  });

  const dialogContentProps = {
    type: DialogType.close,
    title: 'Create new trade',
    subText: 'Select a sticker you would like to receive in exchange for the offered sticker and certain amount of coins.',
  };

  return <Dialog dialogContentProps={dialogContentProps} hidden={false} onDismiss={clearDialog}>
    <Stack className="trading-dialog-content">
      <SwatchColorPicker
        columnCount={10}
        colorCells={colorCells}
        cellShape="square"
        selectedId={selectedId}
        onChange={(_, id) => setSelectedId(id)}
      />
      <SpinButton className="trading-dialog-spin"
        label={coinsValue >= 0 ? "You are offering coins:" : "You are receiving coins:"}
        value={`${coinsValue}`}
        onChange={(_, newValue) => setCoinsValue(Number(newValue))}
        min={-30}
        max={Math.min(30, maxCoins)}
        step={1}
      />
      <DialogFooter>
        <PrimaryButton className="trading-dialog-button"
          onClick={() => onCreateTrade(Number(selectedId), coinsValue)}
          disabled={selectedId === undefined}>
            Offer trade
        </PrimaryButton>
        <DefaultButton className="trading-dialog-secondary-button" onClick={clearDialog}>Cancel</DefaultButton>
      </DialogFooter>
    </Stack>
  </Dialog>
}

export default TradingDialog;
