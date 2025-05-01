import { Category } from './category.enum';
import { CookingDifficulty } from './cookingDifficulty.enum';
import { Tag } from './tags.enum';

export type ShortRecipe = {
    id: string;
    title: string;
    // description: string;
    mainPictureUrl: string;
    cookingTime: number;
    cookingDifficulty: CookingDifficulty;
    calories: number;
    tags: Tag[];
    popularity: number;
    rating: number;
    category: Category;
    isLiked: boolean;
};
