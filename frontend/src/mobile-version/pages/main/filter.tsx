import {
    ButtonLayout,
    FilterContent,
    IngredientsContent,
    Search,
    SortContent,
    SortDirection,
    SortFieldRecipe,
} from '../../components/filter';
import styles from './filter.module.scss';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchRecipes,
    resetFilters,
    selectFilter,
    updateFilter,
} from 'store/main-page.slice';

export function Filter() {
    const filter = useSelector(selectFilter);
    const dispatch = useDispatch();

    const handleSortChange = (
        field: SortFieldRecipe,
        direction: SortDirection
    ) => {
        dispatch(
            updateFilter({
                SortOption: field,
                IsAscendingSorting: direction === 'asc',
            })
        );
    };

    return (
        <div className={styles.mainContainer}>
            <Search
                onChange={(e) => {
                    dispatch(updateFilter({ SearchByTitle: e.target.value }));
                }}
                onClick={() => {
                    dispatch(fetchRecipes());
                }}
                value={filter.SearchByTitle || ''}
            />
            <div className={styles.buttons}>
                <ButtonLayout
                    icon='ingredients'
                    name='Ингредиенты'
                    color='blue'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <IngredientsContent />
                </ButtonLayout>
                <ButtonLayout
                    icon='filter'
                    name='Фильтровать'
                    color='white'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <FilterContent />
                </ButtonLayout>
                <ButtonLayout
                    icon='sort'
                    name='Сортировать'
                    color='white'
                    onClick={() => {
                        dispatch(fetchRecipes());
                    }}
                >
                    <SortContent
                        selectedField={filter.SortOption as SortFieldRecipe}
                        selectedDirection={
                            filter.IsAscendingSorting ? 'asc' : 'desc'
                        }
                        onChange={handleSortChange}
                        isCollection={false}
                    />
                </ButtonLayout>
            </div>
            <button
                className={styles.reset}
                onClick={() => {
                    dispatch(resetFilters());
                    dispatch(fetchRecipes());
                }}
            >
                Сбросить
            </button>
        </div>
    );
}
