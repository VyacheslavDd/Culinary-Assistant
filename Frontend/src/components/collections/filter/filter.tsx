import { ButtonLayout, Search, SortContent } from 'components/filter';
import styles from './filter.module.scss';

export function Filter() {
    return (
        <div className={styles.mainContainer}>
            <Search />
            <div className={styles.buttons}>
                <ButtonLayout icon='sort' name='Сортировать' color='white'>
                    <SortContent />
                </ButtonLayout>
            </div>
        </div>
    );
}
