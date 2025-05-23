import { SortFieldCollection } from 'components/filter';

export type FilterCollection = {
    Title: string;
    Page?: number;
    Limit?: number;
    SortOption: SortFieldCollection;
    IsAscendingSorting: boolean;
};

