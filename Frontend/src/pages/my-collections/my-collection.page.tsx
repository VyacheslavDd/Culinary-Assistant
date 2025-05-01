import { CollList } from 'components/collections';
import styles from './my-collections.module.scss';
import { useSelector } from 'store/store';
import { selectUserCollections } from 'store/user.slice';
import ScrollToTop from 'components/common/scrollToTop';
import { useState } from 'react';

function MyCollectionsPage() {
    const myCollections = useSelector(selectUserCollections);
    const [filter, setFilter] = useState('');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFilter(e.target.value);
    };

    const filteredCollections = myCollections.filter((collection) =>
        collection.title.toLowerCase().includes(filter.toLowerCase())
    );

    return (
        <>
            <ScrollToTop />
            <div className={styles.mainContainer}>
                <CollList
                    title='Мои подборки'
                    collections={filteredCollections}
                    value={filter}
                    onChange={handleChange}
                    isFindShows={false}
                />
            </div>
        </>
    );
}

export default MyCollectionsPage;
