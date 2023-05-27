export interface Basket {
    id: number
    buyerId: string
    items: BasketItem[]
}

export interface BasketItem {
    productId: number
    name: string
    price: number
    pictureUrl: string
    brand: string
    type: any
    quantity: number
}