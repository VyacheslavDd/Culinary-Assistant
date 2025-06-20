import { Best, CollList } from '../../components/collections';
import styles from './collections.module.scss';
import { useDispatch, useSelector } from 'store/store';
import {
    fetchCollections,
    fetchPopularCollections,
    selectCollections,
    selectFilter,
    selectPopularCollections,
    updateFilter,
} from 'store/collections-page.slice';
import { useEffect } from 'react';
import { Helmet } from 'react-helmet-async';

function CollectionsPage() {
    const popularCollections = useSelector(selectPopularCollections);
    const collections = useSelector(selectCollections);
    const dispatch = useDispatch();
    const filter = useSelector(selectFilter);

    useEffect(() => {
        dispatch(fetchCollections());
        dispatch(fetchPopularCollections());
    }, [dispatch]);

    return (
        <div className={styles.mainContainer}>
            <Helmet>
                <title>Все подборки</title>
            </Helmet>
            <Best collections={popularCollections} />
            <CollList
                title='Все подборки'
                collections={collections}
                onChange={(e) =>
                    dispatch(updateFilter({ Title: e.target.value }))
                }
                value={filter.Title}
                onClick={() => dispatch(fetchCollections())}
                selectedField={filter.SortOption}
                selectedDirection={filter.IsAscendingSorting ? 'asc' : 'desc'}
                onSortChange={(sortField, sortDirection) => {
                    dispatch(
                        updateFilter({
                            SortOption: sortField,
                            IsAscendingSorting: sortDirection === 'asc',
                        })
                    );
                    dispatch(fetchCollections());
                }}
            />
        </div>
    );
}

export default CollectionsPage;
