import { ShortUser } from './short-user.type'

export type ShortSelectionRecipe = {
    id: string;
    title: string;
    coverImage: string[];
    author: ShortUser;
}