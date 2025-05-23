import { Category, CookingDifficulty, Tag } from 'types';

export type Filter = {
    SearchByTitle?: string;
    SearchByIngredients: string[];
    StrictIngredientsSearch: boolean;
    CookingTimeFrom?: number;
    CookingTimeTo?: number;
    CookingDifficulties?: CookingDifficulty[];
    Categories?: Category[];
    Tags?: Tag[];
    SortOption: 'byCookingTime' | 'byPopularity' | 'byCalories';
    IsAscendingSorting: boolean;
};
