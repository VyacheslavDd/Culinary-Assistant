import { Color } from './color.enum';
import { ShortRecipe } from './short-recipe.type';
import { ShortUser } from './short-user.type';

export type Collection = {
    id: string;
    title: string;
    isPrivate: boolean;
    isFavourited: false;
    popularity: 0;
    covers: { url: string }[];
    user: ShortUser;
    receipts: ShortRecipe[];
    createdAt: string;
    color: Color;
};
