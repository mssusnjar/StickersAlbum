import urlJoin from "url-join";

const baseUrl = "trades";

export interface TradeOffer
{
    id: string,
    playerId: string;
    offeredStickerId: number;
    wantedStickerId: number;
    coins: number;
    dateCreated: string
}

export const mockTrades: TradeOffer[] = [
    {id: "123", playerId: "player1", offeredStickerId: 11, wantedStickerId: 15, coins: 5, dateCreated: "2024-10-03T09:30:00Z"},
    {id: "124", playerId: "player1", offeredStickerId: 12, wantedStickerId: 13, coins: -5, dateCreated: "2024-10-03T10:30:00Z"},
    {id: "125", playerId: "player1", offeredStickerId: 15, wantedStickerId: 17, coins: 0, dateCreated: "2024-10-03T11:22:00Z"},
    {id: "121", playerId: "player1", offeredStickerId: 7, wantedStickerId: 17, coins: 2, dateCreated: "2024-10-03T11:22:00Z"},
    {id: "129", playerId: "player1", offeredStickerId: 77, wantedStickerId: 27, coins: 20, dateCreated: "2024-10-03T11:22:00Z"}
];

export async function createTrade(username: string, offeredStickerId: number | undefined, wantedStickerId: number | undefined, coins: number): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, "create"), {
        method: "POST",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ PlayerId: username, OfferedStickerId: offeredStickerId, WantedStickerId: wantedStickerId, Coins: coins })
    });

    return response;
}

export async function completeTrade(tradeId: string, ownerPlayerId: string, otherPlayerId: string): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, "complete"), {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ TradeId: tradeId, OwnerPlayerId: ownerPlayerId, OtherPlayerId: otherPlayerId })
    });

    return response;
}

export async function cancelTrade(tradeId: string, ownerPlayerId: string): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, "cancel"), {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ TradeId: tradeId, OwnerPlayerId: ownerPlayerId })
    });

    return response;
}

export async function getActiveTrades(): Promise<TradeOffer[]> {

    let response = await fetch(urlJoin(baseUrl, "list"), {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
        }
    });

    return await response.json() as TradeOffer[];
}

export async function getRecentTradesForUser(username: string): Promise<TradeOffer[]> {

    let response = await fetch(urlJoin(baseUrl, `${username}/list`), {
        method: "GET",
        headers: {
            'Content-Type': 'application/json',
        }
    });

    return await response.json() as TradeOffer[];
}

export async function markTrades(username: string, tradeIds: string[]): Promise<Response> {

    let response = await fetch(urlJoin(baseUrl, `${username}/mark`), {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ TradeIds: tradeIds })
    });

    return response;
}