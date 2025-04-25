import { CollList } from 'components/collections';
import styles from './my-collections.module.scss';

function MyCollectionsPage() {
    return (
        <div className={styles.mainContainer}>
            <CollList title='Мои подборки' />
        </div>
    );
}

export default MyCollectionsPage;
