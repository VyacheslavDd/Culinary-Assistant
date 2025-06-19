import { FilterShort } from '../../components/common/filter/filter';
import styles from './my-recipes.module.scss';
import { Preloader } from '../../components/preloader';
import { Catalog } from '../../components/catalog/catalog';
import { useSelector } from 'store/store';
import { selectUserLoading, selectUserRecipes } from 'store/user.slice';
import { useMemo, useState } from 'react';
import ScrollToTop from '../../components/common/scrollToTop';
import { SortDirection, SortFieldRecipe } from '../../components/filter';
import { Helmet } from 'react-helmet-async';

function MyRecipesPage() {
    const isLoading = useSelector(selectUserLoading);
    const recipes = useSelector(selectUserRecipes);

    const [searchTerm, setSearchTerm] = useState('');
    const [sortField, setSortField] = useState<SortFieldRecipe>('byPopularity');
    const [sortDirection, setSortDirection] = useState<SortDirection>('asc');

    const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setSearchTerm(e.target.value);
    };

    const handleSortChange = (
        field: SortFieldRecipe,
        direction: SortDirection
    ) => {
        setSortField(field);
        setSortDirection(direction);
    };

    const filteredAndSortedRecipes = useMemo(() => {
        const lowerSearch = searchTerm.toLowerCase();

        const filtered = recipes.filter((recipe) =>
            recipe.title.toLowerCase().includes(lowerSearch)
        );

        return [...filtered].sort((a, b) => {
            let comparison = 0;

            if (sortField === 'byPopularity') {
                comparison = a.popularity - b.popularity;
            } else if (sortField === 'byCookingTime') {
                comparison = a.cookingTime - b.cookingTime;
            } else if (sortField === 'byCalories') {
                comparison = a.calories - b.calories;
            }

            return sortDirection === 'asc' ? comparison : -comparison;
        });
    }, [recipes, searchTerm, sortField, sortDirection]);

    return (
        <>
            <ScrollToTop />
            <Helmet>
                <title>Мои рецепты</title>
            </Helmet>
            <main className={styles.main}>
                <h1 className={styles.title}>Мои рецепты</h1>
                <FilterShort
                    onChange={handleSearchChange}
                    value={searchTerm}
                    isFindShows={false}
                    selectedField={sortField}
                    selectedDirection={sortDirection}
                    onSortChange={handleSortChange}
                    isCollection={false}
                />
                {isLoading ? (
                    <Preloader />
                ) : (
                    <Catalog recipes={filteredAndSortedRecipes} />
                )}
            </main>
        </>
    );
}

export default MyRecipesPage;
