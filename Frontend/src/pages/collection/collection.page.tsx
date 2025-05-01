/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { MyCollHeader, OtherCollHeader } from 'components/collection';
import styles from './collection.module.scss';
import { Catalog } from 'components/catalog/catalog';
import ScrollToTop from 'components/common/scrollToTop';
import { useSelector } from 'store/store';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { getCollectionByIdApi } from 'store/api';
import { Collection } from 'types/collections.type';
import { Preloader } from 'components/preloader';
import { selectUser } from 'store/user.slice';

export function CollectionPage() {
    const { id } = useParams<{ id: string }>();
    const [collection, setCollection] = useState<Collection | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const user = useSelector(selectUser);

    const loadCollection = async (id: string) => {
        try {
            setLoading(true);
            const fetchedCollection = await getCollectionByIdApi(id);
            setCollection(fetchedCollection);
        } catch (error) {
            console.error('Error fetching collection:', error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (id) {
            loadCollection(id);
        }
    }, [id]);

    if (loading) {
        return <Preloader />;
    }

    return (
        <>
            <ScrollToTop />
            <div className={styles.mainContainer}>
                {user?.id === collection?.user.id ? (
                    <MyCollHeader collection={collection!} />
                ) : (
                    <OtherCollHeader collection={collection!} />
                )}
                <div className={styles.catalogContainer}>
                    <h2 className={styles.title}> Рецепты</h2>
                    <Catalog recipes={collection?.receipts || []} />
                </div>
            </div>
        </>
    );
}
